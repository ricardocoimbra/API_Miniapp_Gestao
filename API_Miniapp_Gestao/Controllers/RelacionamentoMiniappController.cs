using Microsoft.AspNetCore.Mvc;
using API_Miniapp_Gestao.Services.Interfaces;
using API_Miniapp_Gestao.Audit;
using Swashbuckle.AspNetCore.Annotations;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Constants;

namespace API_Miniapp_Gestao.Controllers
{
   [ApiController]
   [Route(UrlConstants.BaseUrl)]
   [Produces("application/json")]
   public class RelacionamentoMiniAppController : ControllerBase
   {
      private readonly ILogger<RelacionamentoMiniAppController> _logger;
      private readonly IRelacionamentoMiniappService _relacionamentoService;

      private const string InternalServerErrorMessage = "Erro interno do servidor ao processar a solicitação";
      private const string DadosEntradaObrigatoriosMessage = "Dados de entrada são obrigatórios";

      public RelacionamentoMiniAppController(
         ILogger<RelacionamentoMiniAppController> logger,
         IRelacionamentoMiniappService relacionamentoService)
      {
         _logger = logger;
         _relacionamentoService = relacionamentoService;
      }

      /// <summary>
      /// Cadastrar o relacionamento entre miniapps pais e filhos.
      /// </summary>
      /// <param name="entrada">Códigos dos miniapps a serem relacionados</param>
      /// <returns>Sem retorno definido.</returns>
      [HttpPost("relacionamento/criar")]
      [Auditoria(Acao = "INCLUIR_RELACIONAMENTOS_MINIAPPS")]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status204NoContent, "Relacionamento entre miniapps criado com sucesso")]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro salvar o relacionamento entre miniapps")]
      public async Task<IActionResult> PostRelacionamentoMiniapp([FromBody] IncluirRelacionamentoMiniappDto entrada)
      {
         try
         {
            // Validação básica de entrada
            if (entrada == null)
            {
               return BadRequest(new { message = DadosEntradaObrigatoriosMessage });
            }

            if (entrada.CoMiniappPai == Guid.Empty || entrada.CoMiniappFilho == Guid.Empty)
            {
               _logger.LogWarning("GUIDs inválidos - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}",
                  entrada.CoMiniappPai, entrada.CoMiniappFilho);
               return BadRequest(new { message = "Códigos dos mini apps não podem estar vazios" });
            }

            await _relacionamentoService.CriarRelacionamentoAsync(entrada);
            return NoContent();
         }
         catch (BusinessException ex)
         {
            return BadRequest(ex.Erro);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Erro interno ao criar relacionamento entre miniapps");
            return StatusCode(StatusCodes.Status500InternalServerError,
               new { message = InternalServerErrorMessage });
         }
      }

      /// <summary>
      /// Listar relacionamentos de um Miniapp
      /// </summary>
      /// <param name="entrada">Dados de entrada padrão do Miniapp</param>
      /// <returns>Lista com os pai e filhos cadastrados</returns>
      [HttpPost("relacionamento/listar")]
      [Auditoria(Acao = "LISTA_RELACIONAMENTOS_MINIAPP")]
      [SwaggerResponse(StatusCodes.Status200OK, "Relacionamentos dos Miniapps listados com sucesso", typeof(ListaRelacionamentosDto))]
      [SwaggerResponse(StatusCodes.Status404NotFound, "Mini App não encontrado")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao consultar os relacionamentos do Miniapp")]
      public async Task<ActionResult<ListaRelacionamentosDto>> GetRelacionamentosMiniapps([FromBody] EntradaRelacionamentosDto entrada)
      {
         try
         {
            // Validação básica de entrada
            if (entrada == null)
            {
               _logger.LogWarning("Dados de entrada para listagem são nulos");
               return BadRequest(new { message = "Dados de entrada são obrigatórios" });
            }

            if (entrada.CoMiniapp == Guid.Empty)
            {
               _logger.LogWarning("GUID do mini app é inválido para listagem: {CoMiniapp}", entrada.CoMiniapp);
               return BadRequest(new { message = "Código do mini app não pode estar vazio" });
            }

            var resultado = await _relacionamentoService.ListarRelacionamentosAsync(entrada);
            return Ok(resultado);
         }
         catch (BusinessException ex)
         {
            return NotFound(ex.Erro);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Erro ao listar relacionamentos do miniapp {CoMiniapp}", entrada.CoMiniapp);
            return StatusCode(StatusCodes.Status500InternalServerError,
               new { message = "Erro interno do servidor ao processar a solicitação" });
         }
      }

      /// <summary>
      /// Editar o relacionamento entre miniapps pais e filhos.
      /// </summary>
      /// <param name="entrada">Dados para edição do relacionamento (relacionamento original e novos dados)</param>
      /// <returns>Sem retorno definido.</returns>
      [HttpPost("relacionamento/editar")]
      [Auditoria(Acao = "EDITAR_RELACIONAMENTOS_MINIAPPS")]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status204NoContent, "Relacionamento entre miniapps editado com sucesso")]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
      [SwaggerResponse(StatusCodes.Status404NotFound, "Relacionamento original não encontrado")]
      [SwaggerResponse(StatusCodes.Status409Conflict, "Novo relacionamento já existe")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao salvar o relacionamento entre miniapps")]
      public async Task<IActionResult> PutRelacionamentoMiniapp([FromBody] EditarRelacionamentoMiniappDto entrada)
      {
         try
         {
            // Validação básica de entrada
            if (entrada == null)
            {
               _logger.LogWarning("Dados de entrada para edição são nulos");
               return BadRequest(new { message = DadosEntradaObrigatoriosMessage });
            }

            // Validação de GUIDs usando o método do DTO
            if (!entrada.GUIDsValidos)
            {
               _logger.LogWarning("GUIDs inválidos na entrada de edição - Original: Pai: {CoMiniappPaiOriginal}, Filho: {CoMiniappFilhoOriginal} | Novo: Pai: {CoMiniappPaiNovo}, Filho: {CoMiniappFilhoNovo}",
                  entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal, entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
               return BadRequest(new { message = "Todos os códigos dos mini apps devem ser válidos e não podem estar vazios" });
            }

            // Verificar se há mudanças efetivas
            if (!entrada.TemMudancas)
            {
               _logger.LogInformation("Tentativa de edição sem mudanças - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}",
                  entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal);
               return BadRequest(new { message = "Nenhuma alteração foi detectada nos dados do relacionamento" });
            }

            // Validação adicional: evitar autorelacionamento
            if (entrada.CoMiniappPaiNovo == entrada.CoMiniappFilhoNovo)
            {
               _logger.LogWarning("Tentativa de autorelacionamento na edição - MiniApp: {CoMiniapp}", entrada.CoMiniappPaiNovo);
               return BadRequest(new { message = "Um mini app não pode ser pai dele mesmo" });
            }

            await _relacionamentoService.EditarRelacionamentoAsync(entrada);

            _logger.LogInformation("Relacionamento editado com sucesso via controller - Original: Pai: {CoMiniappPaiOriginal}, Filho: {CoMiniappFilhoOriginal} -> Novo: Pai: {CoMiniappPaiNovo}, Filho: {CoMiniappFilhoNovo}",
               entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal, entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);

            return NoContent();
         }
         catch (BusinessException ex)
         {
            _logger.LogWarning(ex, "Erro de negócio ao editar relacionamento");

            // Verificar o tipo de erro para retornar o status apropriado
            var primeiroErro = ex.Erro;
            var codigo = primeiroErro?.Codigo;
            var statusCode = primeiroErro?.StatusCode ?? 400;

            // Log para debug
            _logger.LogDebug("Código da BusinessException: {Codigo}, StatusCode: {StatusCode}", codigo, statusCode);

            // Mapear por StatusCode (usado pelos testes) ou por Codigo (usado pelo service)
            return statusCode switch
            {
               404 => NotFound(ex.Erro),
               409 => Conflict(ex.Erro),
               _ => codigo switch
               {
                  "RELACIONAMENTO_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  "MINIAPP_PAI_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  "MINIAPP_FILHO_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  "RELACIONAMENTO_JA_EXISTE" => Conflict(ex.Erro),
                  _ => BadRequest(ex.Erro)
               }
            };
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Erro interno ao editar relacionamento entre miniapps - Original: Pai: {CoMiniappPaiOriginal}, Filho: {CoMiniappFilhoOriginal}",
               entrada?.CoMiniappPaiOriginal, entrada?.CoMiniappFilhoOriginal);
            return StatusCode(StatusCodes.Status500InternalServerError,
               new { message = InternalServerErrorMessage });
         }
      }
      /// <summary>
      /// Excluir o relacionamento entre miniapps pais e filhos.
      /// </summary>
      /// <param name="entrada">Códigos dos miniapps a excluir o relacionamento</param>
      /// <returns>Sem retorno definido.</returns>
      [HttpPost("relacionamento/excluir")]
      [Auditoria(Acao = "EXCLUIR_RELACIONAMENTOS_MINIAPPS")]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status204NoContent, "Relacionamento entre miniapps excluído com sucesso")]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
      [SwaggerResponse(StatusCodes.Status404NotFound, "Relacionamento não encontrado")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao excluir o relacionamento entre miniapps")]
      public async Task<IActionResult> DeleteRelacionamentoMiniapp([FromBody] IncluirRelacionamentoMiniappDto entrada)
      {
         try
         {
            // Validação básica de entrada
            if (entrada == null)
            {
               _logger.LogWarning("Dados de entrada para exclusão são nulos");
               return BadRequest(new { message = "Dados de entrada são obrigatórios" });
            }

            if (entrada.CoMiniappPai == Guid.Empty || entrada.CoMiniappFilho == Guid.Empty)
            {
               _logger.LogWarning("GUIDs inválidos para exclusão - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}",
                  entrada.CoMiniappPai, entrada.CoMiniappFilho);
               return BadRequest(new { message = "Códigos dos mini apps não podem estar vazios" });
            }

            // Delegar toda a validação e lógica para o service
            await _relacionamentoService.ExcluirRelacionamentoAsync(entrada.CoMiniappPai, entrada.CoMiniappFilho);

            _logger.LogInformation("Relacionamento excluído com sucesso via controller - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}",
               entrada.CoMiniappPai, entrada.CoMiniappFilho);

            return NoContent();
         }
         catch (BusinessException ex)
         {
            _logger.LogWarning(ex, "Erro de negócio ao excluir relacionamento");

            // Verificar o tipo de erro para retornar o status apropriado
            var primeiroErro = ex.Erro;
            var codigo = primeiroErro?.Codigo;
            var statusCode = primeiroErro?.StatusCode ?? 400;

            // Log para debug
            _logger.LogDebug("Código da BusinessException: {Codigo}, StatusCode: {StatusCode}", codigo, statusCode);

            // Mapear por StatusCode (usado pelos testes) ou por Codigo (usado pelo service)
            return statusCode switch
            {
               404 => NotFound(ex.Erro),
               409 => Conflict(ex.Erro),
               _ => codigo switch
               {
                  "RELACIONAMENTO_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  "MINIAPP_PAI_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  "MINIAPP_FILHO_NAO_ENCONTRADO" => NotFound(ex.Erro),
                  _ => BadRequest(ex.Erro)
               }
            };
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Erro interno ao excluir relacionamento entre miniapps - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}",
               entrada?.CoMiniappPai, entrada?.CoMiniappFilho);
            return StatusCode(StatusCodes.Status500InternalServerError,
               new { message = InternalServerErrorMessage });
         }
      }


   }
}
