using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class ListaVersaoSistemaOperacionalMiniappDto
{
    /// <summary>
    /// Código da versão do sistema operacional (filtro opcional)
    /// </summary>
    [JsonPropertyName("coVersaoSistemaOperacional")]
    public Guid? CoVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Código da plataforma (A=Android, I=iOS) - filtro opcional
    /// </summary>
    [StringLength(1, ErrorMessage = "O código da plataforma deve ter exatamente 1 caractere")]
    [RegularExpression("^[AI]$", ErrorMessage = "Código da plataforma deve ser 'A' (Android) ou 'I' (iOS)")]
    [JsonPropertyName("coPlataforma")]
    public string? CoPlataforma { get; set; }

    /// <summary>
    /// Número da versão do sistema operacional (filtro opcional)
    /// </summary>
    [JsonPropertyName("nuVersaoSistemaOperacional")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do sistema operacional deve estar entre 0.001 e 999.999")]
    public decimal? NuVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Número da versão do SDK (filtro opcional)
    /// </summary>
    [JsonPropertyName("nuVersaoSdk")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do SDK deve estar entre 0.001 e 999.999")]
    public decimal? NuVersaoSdk { get; set; }
}
