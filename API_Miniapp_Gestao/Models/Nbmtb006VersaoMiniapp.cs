using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb006VersaoMiniapp
{
    public Guid CoVersaoMiniapp { get; set; }

    public Guid CoMiniapp { get; set; }

    public decimal NuVersaoMiniapp { get; set; }

    public decimal PcExpansaoMiniapp { get; set; }

    public bool IcAtivo { get; set; }

    public DateTime DhInicioVigencia { get; set; }

    public DateTime? DhFimVigencia { get; set; }

    public virtual Nbmtb004Miniapp CoMiniappNavigation { get; set; } = null!;

    public string EdVersaoMiniapp { get; set; } = null!;

    public virtual ICollection<Nbmtb010UsuarioVersaoMiniapp> Nbmtb010UsuarioVersaoMiniapps { get; set; } = new List<Nbmtb010UsuarioVersaoMiniapp>();

    public virtual ICollection<Nbmtb002ModeloDevice> CoModeloDevices { get; set; } = new List<Nbmtb002ModeloDevice>();

    public virtual ICollection<Nbmtb003VersaoAplicativo> CoVersaoAplicativos { get; set; } = new List<Nbmtb003VersaoAplicativo>();

    public virtual ICollection<Nbmtb001VersaoSistemaOperacional> CoVersaoSistemaOperacionals { get; set; } = new List<Nbmtb001VersaoSistemaOperacional>();
}
