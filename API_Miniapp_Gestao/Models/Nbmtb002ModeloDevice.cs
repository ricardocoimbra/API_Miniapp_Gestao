using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb002ModeloDevice
{
    public Guid CoModeloDevice { get; set; }

    public string CoPlataforma { get; set; } = null!;

    public string DeModeloDevice { get; set; } = null!;

    public virtual ICollection<Nbmtb006VersaoMiniapp> CoVersaoMiniapps { get; set; } = new List<Nbmtb006VersaoMiniapp>();
}
