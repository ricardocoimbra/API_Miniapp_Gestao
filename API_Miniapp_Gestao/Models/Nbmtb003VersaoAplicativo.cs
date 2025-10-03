using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb003VersaoAplicativo
{
    public Guid CoVersaoAplicativo { get; set; }

    public decimal NuVersaoAplicativo { get; set; }

    public string CoPlataforma { get; set; } = null!;



    public virtual ICollection<Nbmtb006VersaoMiniapp> CoVersaoMiniapps { get; set; } = new List<Nbmtb006VersaoMiniapp>();
}
