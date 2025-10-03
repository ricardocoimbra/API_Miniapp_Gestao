using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO
{
   /// <summary>
   /// DTO para retorno da lista de miniapps
   /// </summary>
   [ExcludeFromCodeCoverage]
   public class MiniappDto
   {
      [JsonPropertyName("coMiniapp")]
      public Guid CoMiniapp { get; set; }

      [JsonPropertyName("noMiniapp")]
      public string? NoMiniapp { get; set; }

      [JsonPropertyName("noApelidoMiniapp")]
      public string? NoApelidoMiniapp { get; set; }

      [JsonPropertyName("edMiniapp")]
      [NotMapped]
      public string? EdMiniapp { get; set; } = "miniapp.com"; // Valor fixo temporário

      [JsonPropertyName("deMiniapp")]
      public string? DeMiniapp { get; set; }

      [JsonPropertyName("icMiniappInicial")]
      public bool IcMiniappInicial { get; set; }

      [JsonPropertyName("icAtivo")]
      public bool IcAtivo { get; set; }
   }

}
