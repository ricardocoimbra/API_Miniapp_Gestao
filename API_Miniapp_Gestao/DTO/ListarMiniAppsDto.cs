using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO
{

   [ExcludeFromCodeCoverage]
   public class RetornoMiniappDto
   {
      [JsonPropertyName("miniapp")]
      public MiniappDto Miniapp { get; set; } = new();
      [JsonPropertyName("versoesMiniapp")]
      public List<VersaoMiniappDto> VersoesMiniapp { get; set; } = new();
   }

   /// <summary>
   /// DTO para resposta da listagem de miniapps
   /// </summary>
   [ExcludeFromCodeCoverage]
   public class ListarMiniappsResponseDto
   {
      [JsonPropertyName("miniapps")]
      public required IEnumerable<MiniappDto> Miniapps { get; set; }

      [JsonPropertyName("total")]
      public int Total { get; set; }
   }

   /// <summary>
   /// DTO detalhado para miniapp com relacionamentos
   /// </summary>
   [ExcludeFromCodeCoverage]
   public class MiniappDetalhadoDto
   {
      [JsonPropertyName("coMiniapp")]
      public required Guid CoMiniapp { get; set; }

      [JsonPropertyName("noMiniapp")]
      public required string NoMiniapp { get; set; }

      [JsonPropertyName("noApelidoMiniapp")]
      public required string NoApelidoMiniapp { get; set; }

      [JsonPropertyName("edMiniapp")]
      public string? EdMiniapp { get; set; }

      [JsonPropertyName("deMiniapp")]
      public string? DeMiniapp { get; set; }

      [JsonPropertyName("icMiniappInicial")]
      public bool IcMiniappInicial { get; set; }

      [JsonPropertyName("icAtivo")]
      public bool IcAtivo { get; set; }

      [JsonPropertyName("miniappsPai")]
      public IEnumerable<MiniappReferenciaDto>? MiniappsPai { get; set; }

      [JsonPropertyName("miniappsFilhos")]
      public IEnumerable<MiniappReferenciaDto>? MiniappsFilhos { get; set; }
   }

   /// <summary>
   /// DTO para referência simples de miniapp
   /// </summary>
   [ExcludeFromCodeCoverage]
   public class MiniappReferenciaDto
   {
      [JsonPropertyName("coMiniapp")]
      public required Guid CoMiniapp { get; set; }

      [JsonPropertyName("noMiniapp")]
      public required string NoMiniapp { get; set; }

      [JsonPropertyName("noApelidoMiniapp")]
      public required string NoApelidoMiniapp { get; set; }
   }
}
