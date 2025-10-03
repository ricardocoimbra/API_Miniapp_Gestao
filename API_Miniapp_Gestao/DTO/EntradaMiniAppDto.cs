using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO
{
   [ExcludeFromCodeCoverage]
   public class EntradaMiniappDto
   {
      [JsonPropertyName("coMiniapp")]
      public string? CoMiniapp { get; set; } = null;

   }

}