using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Models;

[ExcludeFromCodeCoverage]
public partial class Nbmtb001VersaoSistemaOperacional
{
    [Key]
    public required Guid CoVersaoSistemaOperacional { get; set; }

    [Required(ErrorMessage = "Código da plataforma é obrigatório")]
    [StringLength(1, ErrorMessage = "Código da plataforma deve ter exatamente 1 caractere")]
    [RegularExpression("^[AI]$", ErrorMessage = "Código da plataforma deve ser 'A' (Android) ou 'I' (iOS)")]
    public required string CoPlataforma { get; set; } = null!;

    [Required(ErrorMessage = "Número da versão do sistema operacional é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do sistema operacional deve estar entre 0.001 e 999.999")]
    public required decimal NuVersaoSistemaOperacional { get; set; }

    [Required(ErrorMessage = "Número da versão do SDK é obrigatório")]
    [Range(0.001, 999.999, ErrorMessage = "Versão do SDK deve estar entre 0.001 e 999.999")]
    [NotMapped]
    public required decimal NuVersaoSdk { get; set; }

    public virtual ICollection<Nbmtb006VersaoMiniapp> CoVersaoMiniapps { get; set; } = new List<Nbmtb006VersaoMiniapp>();
}
