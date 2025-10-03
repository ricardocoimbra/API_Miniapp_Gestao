using Microsoft.AspNetCore.Mvc;
using API_Miniapp_Gestao.Services.Interfaces;
using API_Miniapp_Gestao.Audit;
using Swashbuckle.AspNetCore.Annotations;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Constants;
using API_Miniapp_Gestao.Exceptions;

namespace API_Miniapp_Gestao.Controllers
{
   [ApiController]
   [Route(UrlConstants.BaseUrl)]
   [Produces("application/json")]
   public class VersaoMiniappController : ControllerBase
   {
      private readonly ILogger<VersaoMiniappController> _logger;
      private readonly IMiniappVersaoService _miniappVersaoService;

      // Constantes para evitar duplicação de strings literais
      private const string ErroInternoServidor = "Erro interno do servidor";
      private const string ErroInesperado = "Erro inesperado";

      public VersaoMiniappController(ILogger<VersaoMiniappController> logger, IMiniappVersaoService miniappVersaoService)
      {
         _logger = logger;
         _miniappVersaoService = miniappVersaoService;
      }


      /// <summary>
      /// Cadastrar uma nova versão de miniapp
      /// </summary>
      /// <param name="entrada">Dados da versão do miniapp a ser criada (incluindo URL da versão)</param>
      /// <returns>Dados da versão criada</returns>
      [HttpPost("versao/criar")]
      [Auditoria(Acao = "CRIAR_VERSAO_MINIAPP")]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status201Created, "Versão do miniapp criada com sucesso", typeof(RetornoCriarVersaoMiniappDto))]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
      public async Task<IActionResult> PostVersaoMiniapp([FromBody] EntradaCriarVersaoMiniappDto entrada)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         try
         {
            var result = await _miniappVersaoService.CriarVersaoMiniappAsync(entrada);
            return Created($"v1/miniapp/{result.CoMiniapp}/versao/{result.CoVersao}", result);
         }
         catch (KeyNotFoundException ex)
         {
            return NotFound(new { message = ex.Message });
         }
         catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException)
         {
            return NotFound(new { message = ex.InnerException.Message });
         }
         catch (BusinessException ex)
         {
            _logger.LogError(ex, "Erro de negócio ao criar versão do Mini App");

            var primeiroErro = ex.Erro;
            var codigo = primeiroErro?.Codigo ?? "500";

            return codigo switch
            {
               "404" => NotFound(new { message = ex.Message }),
               "409" => Conflict(new { message = ex.Message }),
               "400" => BadRequest(new { message = ex.Message }),
               "500" => StatusCode(500, new { message = ErroInternoServidor }),
               _ => StatusCode(500, new { message = ErroInternoServidor })
            };
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"{ErroInesperado} ao criar versão do Mini App");
            return StatusCode(500, new { message = ErroInternoServidor });
         }
      }

      /// <summary>
      /// Listar versões de um Miniapp
      /// </summary>
      /// <param name="entrada">Dados de entrada padrão do Miniapp</param>
      /// <returns>Lista versões cadastradas </returns>
      [HttpPost("versao/listar")]
      [Auditoria(Acao = "LISTA_RELACIONAMENTOS_MINIAPP")]
      [SwaggerResponse(StatusCodes.Status200OK, "Versões do Miniapp listadas com sucesso", typeof(RetornoListarVersoesDto))]
      [SwaggerResponse(StatusCodes.Status404NotFound, "Mini App não encontrado")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao consultar as versões do Miniapp")]
      public async Task<ActionResult<RetornoListarVersoesDto>> GetVersoesMiniapps([FromBody] EntradaMiniappDto entrada)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         try
         {
            var result = await _miniappVersaoService.ListarVersoesMiniappAsync(entrada);
            return Ok(result);
         }
         catch (ArgumentException ex)
         {
            return BadRequest(new { message = ex.Message });
         }
         catch (KeyNotFoundException ex)
         {
            return NotFound(new { message = ex.Message });
         }
         catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException)
         {
            return NotFound(new { message = ex.InnerException.Message });
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"{ErroInesperado} ao listar versões do miniapp");
            return StatusCode(500, new { message = ErroInternoServidor });
         }
      }

      /// <summary>
      /// Atualizar versões de um Miniapp
      /// </summary>
      /// <param name="entrada">Dados de entrada para atualização da versão</param>
      /// <returns>Versão atualizada</returns>
      [HttpPost("versao/editar")]
      [Auditoria(Acao = "ATUALIZAR_VERSAO_MINIAPP")]
      [SwaggerResponse(StatusCodes.Status200OK, "Versão do Miniapp atualizada com sucesso", typeof(AtualizarVersaoMiniappDto))]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada inválidos")]
      [SwaggerResponse(StatusCodes.Status404NotFound, "Versão ou Mini App não encontrado")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
      public async Task<ActionResult<AtualizarVersaoMiniappDto>> PostAtualizarVersaoMiniapps([FromBody] EntradaAtualizarVersaoMiniappDto entrada)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }

         try
         {
            var result = await _miniappVersaoService.AtualizarVersaoMiniappAsync(entrada);
            return Ok(result);
         }
         catch (ArgumentException ex)
         {
            return BadRequest(new { message = ex.Message });
         }
         catch (KeyNotFoundException ex)
         {
            return NotFound(new { message = ex.Message });
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"{ErroInesperado} ao atualizar versão do miniapp");
            return StatusCode(500, new { message = ErroInternoServidor });
         }
      }

   }

}
