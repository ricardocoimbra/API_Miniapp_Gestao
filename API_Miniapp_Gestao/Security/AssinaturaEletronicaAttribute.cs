using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Security
{
    /// <summary>
    /// Indica que o m�todo da controller requer valida��o de assinatura eletr�nica.
    /// Pode ser utilizado por middlewares ou filtros para aplicar a valida��o necess�ria.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method)]
    public class AssinaturaEletronicaAttribute : Attribute
    {

    }
}
