using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

[ExcludeFromCodeCoverage]
public class AuditoriaConfig
{
    public string ConnectionString { get; set; } = null!;
    public string EventHubName { get; set; } = null!;
    public string NomeAplicacao { get; set; } = null!;
}
