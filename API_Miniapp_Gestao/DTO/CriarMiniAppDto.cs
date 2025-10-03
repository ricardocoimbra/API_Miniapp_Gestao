using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO
{
   [ExcludeFromCodeCoverage]
   public class CriarMiniappDto
   {
      [Required(ErrorMessage = "O nome do miniapp deve ser informado.")]
      [StringLength(200, ErrorMessage = "O nome do miniapp deve ter no máximo 100 caracteres.")]
      [JsonPropertyName("noMiniapp")]
      public string? NoMiniapp { get; set; }

      [Required(ErrorMessage = "O apelido do miniapp deve ser informado.")]
      [StringLength(200, ErrorMessage = "O apelido do miniapp deve ter no máximo 50 caracteres.")]
      [JsonPropertyName("noApelidoMiniapp")]
      public string? NoApelidoMiniapp { get; set; }

      // [Required(ErrorMessage = "O endereço do miniapp deve ser informado.")]
      // [StringLength(200, ErrorMessage = "A edição do miniapp deve ter no máximo 20 caracteres.")]
      // [JsonPropertyName("edMiniapp")]
      // public string? EdMiniapp { get; set; }

      [JsonPropertyName("deMiniapp")]
      public string? DeMiniapp { get; set; } = null;

      [Required(ErrorMessage = "O indicador de miniapp inicial deve ser informado.")]
      [JsonPropertyName("icMiniappInicial")]
      public bool IcMiniappInicial { get; set; } = false;

      [Required(ErrorMessage = "O indicador de ativo deve ser informado.")]
      [JsonPropertyName("icAtivo")]
      public bool IcAtivo { get; set; } = true;
   }

   [ExcludeFromCodeCoverage]
   public class EditarMiniappDto
   {
      [Required(ErrorMessage = "O código do miniapp deve ser informado.")]
      [JsonPropertyName("coMiniapp")]
      public Guid CoMiniapp { get; set; }

      [StringLength(200, ErrorMessage = "O nome do miniapp deve ter no máximo 100 caracteres.")]
      [JsonPropertyName("noMiniapp")]
      public string? NoMiniapp { get; set; }

      [StringLength(200, ErrorMessage = "O apelido do miniapp deve ter no máximo 50 caracteres.")]
      [JsonPropertyName("noApelidoMiniapp")]
      public string? NoApelidoMiniapp { get; set; }

      // [StringLength(200, ErrorMessage = "A edição do miniapp deve ter no máximo 20 caracteres.")]
      // [JsonPropertyName("edMiniapp")]
      // public string? EdMiniapp { get; set; }

      [JsonPropertyName("deMiniapp")]
      public string? DeMiniapp { get; set; }

      [JsonPropertyName("icMiniappInicial")]
      public bool IcMiniappInicial { get; set; }

      [JsonPropertyName("icAtivo")]
      public bool IcAtivo { get; set; }
   }

   [ExcludeFromCodeCoverage]
   public class RetornoCriarMiniappDto
   {
      [JsonPropertyName("coMiniapp")]
      public Guid CoMiniapp { get; set; }

      [JsonPropertyName("noMiniapp")]
      public string NoMiniapp { get; set; } = string.Empty;

      [JsonPropertyName("noApelidoMiniapp")]
      public string NoApelidoMiniapp { get; set; } = string.Empty;

      [JsonPropertyName("deMiniapp")]
      public string? DeMiniapp { get; set; }

      [JsonPropertyName("icMiniappInicial")]
      public bool IcMiniappInicial { get; set; }

      [JsonPropertyName("icAtivo")]
      public bool IcAtivo { get; set; }

      [JsonPropertyName("dataCriacao")]
      public DateTime DataCriacao { get; set; }

      [JsonPropertyName("mensagem")]
      public string Mensagem { get; set; } = "Miniapp criado com sucesso";
   }
}
