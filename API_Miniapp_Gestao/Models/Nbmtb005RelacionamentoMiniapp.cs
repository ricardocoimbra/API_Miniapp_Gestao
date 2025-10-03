using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb005RelacionamentoMiniapp
{
   public Guid CoMiniappPai { get; set; }
   public Guid CoMiniappFilho { get; set; }
}