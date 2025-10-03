using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Atributo que define a ação que será registrada na trilha de auditoria.<br/>
/// Deve ser utilizado em métodos de controllers.<br/>
/// Exemplo de utilização: [Auditoria(Acao = "NOME_DA_ACAO")]
/// </summary>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuditoriaAttribute : Attribute
{
    public string Acao { get; set; } = string.Empty;
}
