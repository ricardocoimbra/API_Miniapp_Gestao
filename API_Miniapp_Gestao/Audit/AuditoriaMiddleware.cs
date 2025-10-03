using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Middleware responsável por capturar as requisições e respostas e enviar os dados para a auditoria.<br/>
/// Também é o responsável por criar o cabeçalho X-Audit-Id, que identifica um conjunto de requisições,
/// ou seja, a requisição ao serviço principal (ex.: API de Consignado) e suas dependências (ex.: API de Elegibilidade).
/// </summary>
[ExcludeFromCodeCoverage]
public class AuditoriaMiddleware(
    ILogger<AuditoriaMiddleware> logger,
    RequestDelegate next,
    TelemetryClient telemetryClient,
    IOptions<AuditoriaConfig> configuration,
    AuditoriaService auditoriaService)
{
    private readonly ILogger<AuditoriaMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    private readonly TelemetryClient _telemetryClient = telemetryClient;
    private readonly string _nomeDaAplicacao = configuration.Value.NomeAplicacao ?? "NAO_INFORMADO";
    private readonly AuditoriaService _auditoriaService = auditoriaService;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<AuditoriaAttribute>() is not null)
        {
            var _timer = new Stopwatch();
            _timer.Start();

            _logger.LogDebug("<<Auditoria>> Entrou AuditoriaMiddleware");
            _logger.LogDebug("<<Auditoria>> Path: {Path}", context.Request.Path.ToString().Replace(Environment.NewLine, ""));

            string acao = "NAO_INFORMADA";
            if (context.GetEndpoint() is not null && context.GetEndpoint()!.Metadata.GetMetadata<AuditoriaAttribute>() is not null)
                acao = context.GetEndpoint()!.Metadata.GetMetadata<AuditoriaAttribute>()!.Acao;

            var xAuditId = context.Request.Headers[AuditoriaConstantes.HeaderAuditKey].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(xAuditId))
            {
                xAuditId = Guid.NewGuid().ToString();
                context.Request.Headers.Append(AuditoriaConstantes.HeaderAuditKey, xAuditId);
            }
            context.Response.Headers.Append(AuditoriaConstantes.HeaderAuditKey, xAuditId);
            _logger.LogInformation("<<Auditoria>> AuditId: {XAuditId}", xAuditId.Replace(Environment.NewLine, ""));

            context.Request.EnableBuffering();

            context.Items.Add(AuditoriaConstantes.Acao, acao);
            context.Items.Add(AuditoriaConstantes.NomeAplicacao, _nomeDaAplicacao);

            _telemetryClient.TrackTrace("TRILHA_AUDITORIA");
            using var requestBodyStream = new MemoryStream();

            var originalRequestBody = context.Request.Body;

            string stringRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            HttpResponse response = context.Response;
            var originalResponseBody = response.Body;
            using var newResponseBody = new MemoryStream();
            response.Body = newResponseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogDebug("<<Auditoria>> Entrou EnviarParaAuditoria");
                await EnviarParaAuditoria(context, newResponseBody, stringRequestBody, originalResponseBody, acao, _timer);
                context.Request.Body = originalRequestBody;
                context.Response.Body = originalResponseBody;
                _logger.LogDebug("<<Auditoria>> Saiu EnviarParaAuditoria");
            }

            _logger.LogDebug("<<Auditoria>> Saiu AuditoriaMiddleware;");
        }
        else
        {
            await _next(context);
        }
    }

    /// <summary>
    /// Prepara os dados da requisição e resposta e os envia para auditoria.
    /// </summary>
    private async Task EnviarParaAuditoria(
        HttpContext context,
        Stream responseBody,
        string stringRequestBody,
        Stream originalResponseBody,
        string acao,
        Stopwatch timer)
    {
        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            string? stringResponseBody = await new StreamReader(responseBody).ReadToEndAsync();

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);

            timer.Stop();

            var auditoriaDTO = new AuditoriaDto(stringRequestBody, stringResponseBody, context, acao, _nomeDaAplicacao, tempoResposta: timer.ElapsedMilliseconds);

            _telemetryClient.TrackTrace("ENVIO_TRILHA_AUDITORIA", new Dictionary<string, string>()
            {
                { "rawJson", JsonSerializer.Serialize(auditoriaDTO) }
            });

            // Retorno (incluindo exceções) é descartado.
            _ = _auditoriaService.SendAsync(auditoriaDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<<Auditoria>> Exception capturada durante o preparo dos dados a serem enviados.");
            _telemetryClient.TrackTrace("ERRO_ENVIO_TRILHA_AUDITORIA", new Dictionary<string, string>()
            {
                { "erro", ex.Message }
            });
        }
    }
}
