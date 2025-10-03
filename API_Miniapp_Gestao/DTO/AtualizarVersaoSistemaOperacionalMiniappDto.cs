using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class AtualizarVersaoSistemaOperacionalMiniappDto
{
    /// <summary>
    /// Código da versão do sistema operacional. Obrigatório para identificar qual versão atualizar.
    /// </summary>
    [JsonPropertyName("coVersaoSistemaOperacional")]
    [Required(ErrorMessage = "Código da versão do sistema operacional é obrigatório")]
    public required Guid CoVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Código da plataforma (A=Android, I=iOS)
    /// </summary>
    [Required(ErrorMessage = "O código da plataforma é obrigatório")]
    [StringLength(1, ErrorMessage = "O código da plataforma deve ter exatamente 1 caractere")]
    [RegularExpression("^[AI]$", ErrorMessage = "Código da plataforma deve ser 'A' (Android) ou 'I' (iOS)")]
    [JsonPropertyName("coPlataforma")]
    public required string CoPlataforma { get; set; }

    /// <summary>
    /// Número da versão do sistema operacional
    /// </summary>
    [Required(ErrorMessage = "Número da versão do sistema operacional é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do sistema operacional deve estar entre 0.001 e 999.999")]
    [JsonPropertyName("nuVersaoSistemaOperacional")]
    public required decimal NuVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Número da versão do SDK
    /// </summary>
    [Required(ErrorMessage = "Número da versão do SDK é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do SDK deve estar entre 0.001 e 999.999")]
    [JsonPropertyName("nuVersaoSdk")]
    public required decimal NuVersaoSdk { get; set; }
}
