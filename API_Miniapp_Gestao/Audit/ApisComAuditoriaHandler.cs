using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Handler responsável por tratar requisições a APIs do NCR (e que utilizam a biblioteca de auditoria).<br/>
/// Estas requisições receberão o cabeçalho X-Audit-Id referente a requisição principal, mas o envio dos dados
/// para auditoria será feito pela própria API requisitada.<br/>
/// O middleware de auditoria é o responsável por criar o cabeçalho X-Audit-Id.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApisComAuditoriaHandler(
    ILogger<ApisComAuditoriaHandler> logger,
    IHttpContextAccessor contextAccessor) : DelegatingHandler
{
    private readonly ILogger<ApisComAuditoriaHandler> _logger = logger;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    /// <summary>
    /// 1. Captura o cabeçalho X-Audit-Id da requisição principal e o adiciona à requisição subsequente.<br/>
    /// 2. Procede com a requisição subsequente, sem tratar sua resposta.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("<<Auditoria>> Entrou ApisComAuditoriaHandler");
        _logger.LogDebug("<<Auditoria>> RequestUri: {RequestUri}", request.RequestUri?.ToString().Replace(Environment.NewLine, ""));

        var headerAuditKey = _contextAccessor.HttpContext?.Request.Headers[AuditoriaConstantes.HeaderAuditKey];
        if (headerAuditKey.HasValue)
        {
            _logger.LogInformation("<<Auditoria>> AuditId propagado: {XAuditId}", headerAuditKey.Value.ToString().Replace(Environment.NewLine, ""));
            request.Headers.Add(AuditoriaConstantes.HeaderAuditKey, headerAuditKey.Value.ToString());
        }

        _logger.LogDebug("<<Auditoria>> Saiu ApisComAuditoriaHandler");

        return await base.SendAsync(request, cancellationToken);
    }
}
