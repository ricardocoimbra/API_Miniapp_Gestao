using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class EntradaAtualizarVersaoMiniappDto
{
    [JsonPropertyName("coVersao")]
    public Guid CoVersao { get; set; }

    [JsonPropertyName("coMiniapp")]
    public Guid CoMiniapp { get; set; }

    [JsonPropertyName("nuVersao")]
    public decimal? NuVersao { get; set; }

    [JsonPropertyName("pcExpansao")]
    public decimal? PcExpansao { get; set; }

    [JsonPropertyName("icAtivo")]
    public bool IcAtivo { get; set; }

    [JsonPropertyName("edVersaoMiniapp")]
    public string? EdVersaoMiniapp { get; set; }
}
