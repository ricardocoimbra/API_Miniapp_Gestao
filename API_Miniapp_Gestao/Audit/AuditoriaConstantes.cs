using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

[ExcludeFromCodeCoverage]
public static class AuditoriaConstantes
{
    public static readonly string HeaderAuditKey = "X-Audit-Id";
    public static readonly string Acao = "Acao";
    public static readonly string NomeAplicacao = "NomeAplicacao";
}
