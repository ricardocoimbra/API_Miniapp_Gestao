using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class VersaoMiniappDto
{
   [JsonPropertyName("coVersao")]
   public Guid CoVersao { get; set; }

   [JsonPropertyName("nuVersao")]
   public decimal? NuVersao { get; set; }

   [JsonPropertyName("pcExpansao")]
   public decimal? PcExpansao { get; set; }
   [JsonPropertyName("icAtivo")]
   public bool IcAtivo { get; set; }

   [JsonPropertyName("edVersaoMiniapp")]
   public string? EdVersaoMiniapp { get; set; }
}

[ExcludeFromCodeCoverage]
public class EntradaCriarVersaoMiniappDto
{
   [Required]
   public Guid CoMiniapp { get; set; }
   
   public decimal? NuVersao { get; set; }
   
   public decimal? PcExpansao { get; set; }
   
   public bool IcAtivo { get; set; }
   
   [Url]
   public string? EdVersaoMiniapp { get; set; }
}

[ExcludeFromCodeCoverage]
public class RetornoCriarVersaoMiniappDto
{
   public Guid CoVersao { get; set; }
   public Guid CoMiniapp { get; set; }
   public decimal? NuVersao { get; set; }
   public decimal? PcExpansao { get; set; }
   public bool IcAtivo { get; set; }
   public string? EdVersaoMiniapp { get; set; }
}

[ExcludeFromCodeCoverage]
public class RetornoListarVersoesDto
{
   public List<VersaoMiniappDto> RetornoListaVersoes { get; set; } = new();
}

public class EntradaEditarVersaoMiniappDto
{
   [Required]
   public Guid CoVersaoMiniapp { get; set; }
   
   [Required]
   public Guid CoMiniapp { get; set; }
   
   public decimal? NuVersaoMiniapp { get; set; }
   
   public decimal? PcExpansaoMiniapp { get; set; }
   
   public string? EdVersaoMiniapp { get; set; }
   
   public bool IcAtivo { get; set; }
}

public class RetornoEditarVersaoMiniappDto
{
   public Guid CoVersaoMiniapp { get; set; }
   public Guid CoMiniapp { get; set; }
   public decimal? NuVersaoMiniapp { get; set; }
   public decimal? PcExpansaoMiniapp { get; set; }
   public string? EdVersaoMiniapp { get; set; }
   public bool IcAtivo { get; set; }
}
