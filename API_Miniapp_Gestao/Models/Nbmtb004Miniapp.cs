using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb004Miniapp
{
    public Guid CoMiniapp { get; set; }

    public string NoMiniapp { get; set; } = null!;

    public string NoApelidoMiniapp { get; set; } = null!;



    public string? DeMiniapp { get; set; }

    public bool IcMiniappInicial { get; set; }

    public bool IcAtivo { get; set; }

    public virtual ICollection<Nbmtb006VersaoMiniapp> Nbmtb006VersaoMiniapps { get; set; } = new List<Nbmtb006VersaoMiniapp>();

    public virtual ICollection<Nbmtb004Miniapp> CoMiniappFilhos { get; set; } = new List<Nbmtb004Miniapp>();

    public virtual ICollection<Nbmtb004Miniapp> CoMiniappPais { get; set; } = new List<Nbmtb004Miniapp>();
}
