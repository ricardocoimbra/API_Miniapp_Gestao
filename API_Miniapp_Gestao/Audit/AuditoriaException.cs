using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

[ExcludeFromCodeCoverage]
public class AuditoriaException(string message) : Exception(message)
{
    public override string ToString()
    {
        return $"AuditoriaException: {Message}";
    }
}
