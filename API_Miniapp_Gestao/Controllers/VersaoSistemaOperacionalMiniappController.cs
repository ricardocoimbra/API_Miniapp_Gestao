using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using API_Miniapp_Gestao.Audit;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Constants;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Services.Interfaces;


namespace API_Miniapp_Gestao.Controllers
{
    [ApiController]
    [Route(UrlConstants.BaseUrl)]
    [Produces("application/json")]
    public class VersaoSistemaOperacionalMiniappController : ControllerBase
    {
        private readonly ILogger<VersaoSistemaOperacionalMiniappController> _logger;
        private readonly IVersaoSistemaOperacionalMiniappService _versaoSistemaOperacionalService;

        public VersaoSistemaOperacionalMiniappController(
            ILogger<VersaoSistemaOperacionalMiniappController> logger,
            IVersaoSistemaOperacionalMiniappService versaoSistemaOperacionalService)
        {
            _logger = logger;
            _versaoSistemaOperacionalService = versaoSistemaOperacionalService;
        }

        [HttpPost("sistema-operacional/criar")]
        [Auditoria(Acao = "CRIAR_VERSAO_SO_MINIAPP")]
        [SwaggerResponse(StatusCodes.Status201Created, "Versão do sistema operacional criada com sucesso", typeof(RetornoVersaoSistemaOperacionalMiniappDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
        public async Task<IActionResult> PostVersaoSistemaOperacionalMiniapp([FromBody] EntradaVersaoSistemaOperacionalMiniappDto entrada)
        {
                if (!ModelState.IsValid)
            {
                throw new BusinessException("VALIDATION_ERROR", "Dados de entrada inválidos", 400);
            }


            var resultado = await _versaoSistemaOperacionalService.CriarVersaoSistemaOperacionalAsync(entrada);
            return Created($"v1/sistema-operacional/{resultado.CoVersaoSistemaOperacional}", resultado);

        }

        [HttpPost("sistema-operacional/listar")]
        [Auditoria(Acao = "LISTAR_VERSAO_SO_MINIAPP")]
        [SwaggerResponse(StatusCodes.Status200OK, "Versões do sistema operacional listadas com sucesso", typeof(List<ListaRetornoVersaoSistemaOperacionalMiniappDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
        public async Task<IActionResult> GetVersaoSistemaOperacionalMiniapp([FromBody] ListaVersaoSistemaOperacionalMiniappDto? entrada = null)
        {

            var resultado = await _versaoSistemaOperacionalService.ListarVersoesSistemaOperacionalAsync(entrada ?? new ListaVersaoSistemaOperacionalMiniappDto());
            return Ok(resultado);
        }

        [HttpPost("sistema-operacional/editar")]
        [Auditoria(Acao = "ATUALIZAR_VERSAO_SO_MINIAPP")]
        [SwaggerResponse(StatusCodes.Status200OK, "Versão do sistema operacional atualizada com sucesso", typeof(RetornoVersaoSistemaOperacionalMiniappDto))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Versão do sistema operacional não encontrada")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
        public async Task<IActionResult> PatchVersaoSistemaOperacionalMiniapp([FromBody] AtualizarVersaoSistemaOperacionalMiniappDto entrada)
        {
            if (!ModelState.IsValid)
            {
                throw new BusinessException("VALIDATION_ERROR", "Dados de entrada inválidos", 400);
            }

            var resultado = await _versaoSistemaOperacionalService.AtualizarVersaoSistemaOperacionalAsync(entrada);
            return Ok(resultado);
        }

        [HttpPost("sistema-operacional/excluir")]
        [Auditoria(Acao = "EXCLUIR_VERSAO_SO_MINIAPP")]
        [SwaggerResponse(StatusCodes.Status200OK, "Versão do sistema operacional excluída com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados de entrada incompletos ou inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Versão do sistema operacional não encontrada")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
        public async Task<IActionResult> DeleteVersaoSistemaOperacionalMiniapp([FromBody] RemoveVersaoSistemaOperacionalMiniappDto entrada)
        {
            if (!ModelState.IsValid)
            {
                throw new BusinessException("VALIDATION_ERROR", "Dados de entrada inválidos", 400);
            }

            await _versaoSistemaOperacionalService.ExcluirVersaoSistemaOperacionalAsync(entrada);
            return Ok(new { message = "Versão do sistema operacional excluída com sucesso" });

        }
    }
}
