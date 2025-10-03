using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class IncluirRelacionamentoMiniappDto
{
   public Guid CoMiniappPai { get; set; }
   public Guid CoMiniappFilho { get; set; }
}

[ExcludeFromCodeCoverage]
public class EntradaRelacionamentosDto
{
   public string Relacao { get; set; } = null!;
   public Guid CoMiniapp { get; set; }
}

[ExcludeFromCodeCoverage]
public class RetornoRelacionamentosDto
{
   public Guid CoMiniapp { get; set; }
   public string? NoMiniapp { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListaRelacionamentosDto
{
   public List<RetornoRelacionamentosDto> pais { get; set; } = new();
   public List<RetornoRelacionamentosDto> filhos { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public class EditarRelacionamentoMiniappDto
{
   /// <summary>
   /// Código do miniapp pai original (antes da edição)
   /// </summary>
   public Guid CoMiniappPaiOriginal { get; set; }

   /// <summary>
   /// Código do miniapp filho original (antes da edição)
   /// </summary>
   public Guid CoMiniappFilhoOriginal { get; set; }

   /// <summary>
   /// Código do novo miniapp pai (após a edição)
   /// </summary>
   public Guid CoMiniappPaiNovo { get; set; }

   /// <summary>
   /// Código do novo miniapp filho (após a edição)
   /// </summary>
   public Guid CoMiniappFilhoNovo { get; set; }

   /// <summary>
   /// Verifica se há mudanças efetivas nos relacionamentos
   /// </summary>
   public bool TemMudancas =>
      CoMiniappPaiOriginal != CoMiniappPaiNovo ||
      CoMiniappFilhoOriginal != CoMiniappFilhoNovo;

   /// <summary>
   /// Verifica se todos os GUIDs são válidos
   /// </summary>
   public bool GUIDsValidos =>
      CoMiniappPaiOriginal != Guid.Empty &&
      CoMiniappFilhoOriginal != Guid.Empty &&
      CoMiniappPaiNovo != Guid.Empty &&
      CoMiniappFilhoNovo != Guid.Empty;
}