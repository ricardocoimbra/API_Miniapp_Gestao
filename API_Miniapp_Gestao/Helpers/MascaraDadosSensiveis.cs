using API_Miniapp_Gestao.Audit;
using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MascaraDadosSensiveis
    {
        public AuditoriaDto MascararDados(AuditoriaDto auditoriaDto)
        {
            return auditoriaDto;
        }
    }
}
