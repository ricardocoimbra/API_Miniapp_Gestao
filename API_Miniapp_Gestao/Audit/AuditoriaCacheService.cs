using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Serviço para envio de dados armazenados em cache para auditoria.<br/>
/// Exemplo de utilização: Ao recuperar um valor válido do cache, chamar o método
/// EnviarParaAuditoria informando a chave do cache, o valor recuperado e a URL da
/// requisição (caso se trate de uma chamada à alguma API).
/// </summary>
[ExcludeFromCodeCoverage]
public class AuditoriaCacheService(
    ILogger<AuditoriaCacheService> logger,
    AuditoriaService auditoriaService,
    IHttpContextAccessor contextAccessor,
    TelemetryClient telemetryClient)
{
    private readonly ILogger<AuditoriaCacheService> _logger = logger;
    private readonly AuditoriaService _auditoriaService = auditoriaService;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    /// <summary>
    /// Prepara os dados de cache e os envia para auditoria.
    /// </summary>
    public void EnviarParaAuditoria(string key, string? cache, string? url, long? tempoResposta = null)
    {
        if (string.IsNullOrWhiteSpace(cache)) return;

        try
        {
            var headers = new Dictionary<string, string[]>();
            var headerAuditKey = _contextAccessor.HttpContext?.Request.Headers[AuditoriaConstantes.HeaderAuditKey];

            headers.Add(AuditoriaConstantes.HeaderAuditKey, [headerAuditKey.ToString() ?? ""]);
            headers.Add("key", [key]);

            var acao = _contextAccessor.HttpContext?.Items[AuditoriaConstantes.Acao]?.ToString() ?? string.Empty;
            var nomeAplicacao = _contextAccessor.HttpContext?.Items[AuditoriaConstantes.NomeAplicacao]?.ToString() ?? string.Empty;

            AuditoriaDto auditoriaDto;
            if (string.IsNullOrWhiteSpace(url))
            {
                auditoriaDto = new("", "CACHE", "", headers, "", "200", cache, headers, acao, nomeAplicacao, "", key, tempoResposta: tempoResposta);
            }
            else
            {
                var uri = new Uri(url);
                auditoriaDto = new(uri.PathAndQuery, "CACHE", uri.Host, headers, "", "200", cache, headers, acao, nomeAplicacao, "", key, tempoResposta: tempoResposta);
            }

            // Retorno (incluindo exceções) é descartado.
            _ = _auditoriaService.SendAsync(auditoriaDto);
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
