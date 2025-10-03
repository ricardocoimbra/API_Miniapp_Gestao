using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb010UsuarioVersaoMiniapp
{
    public Guid CoUsuarioVersaoMiniapp { get; set; }

    public Guid CoVersaoMiniapp { get; set; }

    public decimal NuCpfUsuario { get; set; }

    public virtual Nbmtb006VersaoMiniapp CoVersaoMiniappNavigation { get; set; } = null!;
}
