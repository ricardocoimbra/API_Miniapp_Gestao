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
    public class MiniAppController : ControllerBase
    {
        private readonly IMiniAppService _miniAppGestaoService;

        public MiniAppController(IMiniAppService miniAppGestaoService)
        {
            _miniAppGestaoService = miniAppGestaoService;
        }

        /// <summary>
        /// Listar Miniapps cadastrados. Quando o parâmetro CoMiniapp é fornecido, retorna apenas o Miniapp correspondente.
        /// </summary>
        /// <param name="entrada">CoMiniapp a ser consultado (opcional)</param>
        /// <returns>Lista com os Miniapps cadastrados ou o Miniapp solicitado pelo CoMiniapp.</returns>
        [HttpPost("listar")]
        [Auditoria(Acao = "LISTAR_MINIAPP")]
        [SwaggerResponse(StatusCodes.Status200OK, "Miniapps listados com sucesso", typeof(List<RetornoMiniappDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Mini App não encontrado")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao consultar a lista de Miniapps")]
        public async Task<ActionResult<List<RetornoMiniappDto>>> GetListarMiniapps([FromBody] EntradaMiniappDto entrada = null!)
        {
            var result = await _miniAppGestaoService.ConsultaMiniapps(entrada);
            return result == null || !result.Any()
                        ? NotFound(new { message = "Mini App não encontrado." })
                        : Ok(result);

        }

        /// <summary>
        /// Criar um novo miniapp
        /// </summary>
        /// <param name="entrada">Dados do miniapp a ser criado</param>
        /// <returns>Dados do miniapp criado</returns>
        [HttpPost("criar")]
        [Auditoria(Acao = "CRIAR_MINIAPP")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, "Miniapp criado com sucesso", typeof(MiniappDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados fornecidos incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao criar o miniapp")]
        public async Task<IActionResult> PostMiniapp([FromBody] CriarMiniappDto entrada)
        {
            // Validação se o nome do miniapp já existe
            var existingMiniapp = await _miniAppGestaoService.MiniappExists(entrada.NoMiniapp!);
            if (existingMiniapp) return Conflict(new { message = "Nome do miniapp já existe." });
            // Validação dos dados de entrada
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdMiniapp = await _miniAppGestaoService.CriarMiniapp(entrada);
                return StatusCode(StatusCodes.Status201Created, createdMiniapp);
            }
            catch (BusinessException)
            {
                throw new BusinessException("Criar Miniapp", "Erro ao criar Miniapp", 500);
            }
        }

        /// <summary>
        /// Editar um dado miniapp
        /// </summary>
        /// <param name="entrada">Dados do miniapp a ser editado</param>
        /// <returns>Dados do miniapp editado</returns>
        [HttpPost("editar")]
        [Auditoria(Acao = "EDITAR_MINIAPP")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, "Miniapp editado com sucesso", typeof(MiniappDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados fornecidos incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Miniapp não encontrado")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro ao editar o miniapp")]
        public async Task<IActionResult> PutMiniapp([FromBody] EditarMiniappDto entrada)
        {
            throw new NotImplementedException();
        }
    }
}


