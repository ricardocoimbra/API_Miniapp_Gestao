using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using API_Miniapp_Gestao.Models;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class EntradaVersaoSistemaOperacionalMiniappDto
{
    /// <summary>
    /// Código da versão do sistema operacional (GUID)
    /// </summary>
    [JsonPropertyName("coVersaoSistemaOperacional")]
    public Guid? CoVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Código da plataforma (A=Android, I=iOS)
    /// </summary>
    [JsonPropertyName("coPlataforma")]
    [Required(ErrorMessage = "O código da plataforma é obrigatório")]
    [StringLength(1, ErrorMessage = "Código da plataforma deve ter exatamente 1 caractere")]
    [RegularExpression("^[AI]$", ErrorMessage = "Código da plataforma deve ser 'A' (Android) ou 'I' (iOS)")]
    public required string CoPlataforma { get; set; } = null!;

    /// <summary>
    /// Número da versão do sistema operacional
    /// </summary>
    [JsonPropertyName("nuVersaoSistemaOperacional")]
    [Required(ErrorMessage = "Número da versão do sistema operacional é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do sistema operacional deve estar entre 0.001 e 999.999")]
    public required decimal NuVersaoSistemaOperacional { get; set; }

    /// <summary>
    /// Número da versão do SDK
    /// </summary>
    [JsonPropertyName("nuVersaoSdk")]
    [Required(ErrorMessage = "Número da versão do SDK é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do SDK deve estar entre 0.001 e 999.999")]
    public required decimal NuVersaoSdk { get; set; }
}

[ExcludeFromCodeCoverage]
public class RetornoVersaoSistemaOperacionalMiniappDto
{
    [JsonPropertyName("coVersaoSistemaOperacional")]
    public Guid CoVersaoSistemaOperacional { get; set; }

    [JsonPropertyName("coPlataforma")]
    public string CoPlataforma { get; set; } = null!;

    [JsonPropertyName("nuVersaoSistemaOperacional")]
    public decimal NuVersaoSistemaOperacional { get; set; }

    [JsonPropertyName("nuVersaoSdk")]
    public decimal NuVersaoSdk { get; set; }
}

[ExcludeFromCodeCoverage]
public class RemoveVersaoSistemaOperacionalMiniappDto
{
    [JsonPropertyName("coVersaoSistemaOperacional")]
    public required Guid CoVersaoSistemaOperacional { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListaRetornoVersaoSistemaOperacionalMiniappDto
{
    [JsonPropertyName("coVersaoSistemaOperacional")]
    public Guid CoVersaoSistemaOperacional { get; set; }

    [JsonPropertyName("coPlataforma")]
    public string CoPlataforma { get; set; } = null!;

    [JsonPropertyName("nuVersaoSistemaOperacional")]
    public decimal NuVersaoSistemaOperacional { get; set; }

    [JsonPropertyName("nuVersaoSdk")]
    public decimal NuVersaoSdk { get; set; }
}
