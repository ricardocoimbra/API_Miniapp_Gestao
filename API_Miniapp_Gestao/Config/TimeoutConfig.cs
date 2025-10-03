using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Config
{
    [ExcludeFromCodeCoverage]
    public class TimeoutConfig
    {
        public double TimeoutSaldoMilissegundos { get; set; }
        public double TimeoutPadraoMilissegundos { get; set; }
    }
}
