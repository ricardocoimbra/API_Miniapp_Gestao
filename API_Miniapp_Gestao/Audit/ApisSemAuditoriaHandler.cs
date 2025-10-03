using Microsoft.ApplicationInsights;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Handler responsável por tratar requisições a APIs que estão fora do escopo do NCR
/// (ou seja, não utilizam a biblioteca de auditoria).<br/>
/// Estas requisições receberão o cabeçalho X-Audit-Id referente a requisição principal
/// e os dados de auditoria serão enviados para o Azure Event Hubs.<br/>
/// O middleware de auditoria é o responsável por criar o cabeçalho X-Audit-Id.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApisSemAuditoriaHandler(
    ILogger<ApisSemAuditoriaHandler> logger,
    AuditoriaService auditoriaService,
    IHttpContextAccessor contextAccessor,
    TelemetryClient telemetryClient) : DelegatingHandler
{
    private readonly ILogger<ApisSemAuditoriaHandler> _logger = logger;
    private readonly AuditoriaService _auditoriaService = auditoriaService;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    /// <summary>
    /// 1. Captura o cabeçalho X-Audit-Id da requisição principal e o adiciona à requisição subsequente.<br/>
    /// 2. Procede com a requisição subsequente e aguarda resposta.<br/>
    /// 3. Envia os dados da resposta para auditoria.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var _timer = new Stopwatch();
        _timer.Start();
        _logger.LogDebug("<<Auditoria>> Entrou ApisSemAuditoriaHandler");
        _logger.LogDebug("<<Auditoria>> RequestUri: {RequestUri}", request.RequestUri?.ToString().Replace(Environment.NewLine, ""));

        var headerAuditKey = _contextAccessor.HttpContext?.Request.Headers[AuditoriaConstantes.HeaderAuditKey];
        if (headerAuditKey.HasValue)
        {
            _logger.LogInformation("<<Auditoria>> AuditId propagado: {XAuditId}", headerAuditKey.Value.ToString().Replace(Environment.NewLine, ""));
            request.Headers.Add(AuditoriaConstantes.HeaderAuditKey, headerAuditKey.Value.ToString());
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        _logger.LogDebug("<<Auditoria>> Entrou EnviarParaAuditoria");
        await EnviarParaAuditoria(request, response, cancellationToken, _timer);
        _logger.LogDebug("<<Auditoria>> Saiu EnviarParaAuditoria");

        return response;
    }

    /// <summary>
    /// Prepara os dados da requisição e resposta e os envia para auditoria.
    /// </summary>
    private async Task EnviarParaAuditoria(
        HttpRequestMessage request,
        HttpResponseMessage response,
        CancellationToken cancellationToken,
        Stopwatch timer)
    {
        try
        {
            string requestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var acao = _contextAccessor.HttpContext?.Items[AuditoriaConstantes.Acao]?.ToString() ?? string.Empty;
            var nomeAplicacao = _contextAccessor.HttpContext?.Items[AuditoriaConstantes.NomeAplicacao]?.ToString()
                ?? string.Empty;
            timer.Stop();
            AuditoriaDto auditoriaDTO = new(request, requestBody, response, responseBody, acao, nomeAplicacao, tempoResposta: timer.ElapsedMilliseconds);
            _logger.LogDebug("<<Auditoria>> ApisSemAuditoriaHandler Ação: {Acao} | Nome Aplicação: {NomeAplicacao}",
                acao, nomeAplicacao);

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
