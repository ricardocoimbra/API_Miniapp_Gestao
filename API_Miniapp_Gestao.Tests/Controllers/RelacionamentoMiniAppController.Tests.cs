using API_Miniapp_Gestao.Controllers;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API_Miniapp_Gestao.Tests.Controllers;

public class RelacionamentoMiniAppControllerTests
{
    private readonly Mock<ILogger<RelacionamentoMiniAppController>> _loggerMock = new();
    private readonly Mock<IRelacionamentoMiniappService> _serviceMock = new();
    private readonly RelacionamentoMiniAppController _controller;

    public RelacionamentoMiniAppControllerTests()
    {
        _controller = new RelacionamentoMiniAppController(_loggerMock.Object, _serviceMock.Object);
    }

    [Fact]
    public async Task PostRelacionamentoMiniapp_DeveRetornarNoContent_QuandoDadosValidos()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };

        // Act
        var result = await _controller.PostRelacionamentoMiniapp(entrada);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.CriarRelacionamentoAsync(entrada), Times.Once);
    }

    [Fact]
    public async Task PostRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoEntradaNula()
    {
        var result = await _controller.PostRelacionamentoMiniapp(null!);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("obrigat", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoGuidsVazios()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.Empty, CoMiniappFilho = Guid.Empty };

        // Act
        var result = await _controller.PostRelacionamentoMiniapp(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("vazios", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoBusinessException()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };
        var erro = new ErroDto { Codigo = "400", Mensagem = "Erro de negócio" };
        _serviceMock.Setup(s => s.CriarRelacionamentoAsync(entrada)).ThrowsAsync(new BusinessException(erro));

        // Act
        var result = await _controller.PostRelacionamentoMiniapp(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(erro, badRequest.Value);
    }

    [Fact]
    public async Task PostRelacionamentoMiniapp_DeveRetornarStatus500_QuandoExceptionGenerica()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };
        _serviceMock.Setup(s => s.CriarRelacionamentoAsync(entrada)).ThrowsAsync(new Exception("Erro interno"));

        // Act
        var result = await _controller.PostRelacionamentoMiniapp(entrada);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        Assert.Contains("Erro interno", statusResult.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetRelacionamentosMiniapps_DeveRetornarOk_QuandoDadosValidos()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto { CoMiniapp = Guid.NewGuid(), Relacao = "AMBOS" };
        var esperado = new ListaRelacionamentosDto();
        _serviceMock.Setup(s => s.ListarRelacionamentosAsync(entrada)).ReturnsAsync(esperado);

        // Act
        var result = await _controller.GetRelacionamentosMiniapps(entrada);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(esperado, ok.Value);
    }

    [Fact]
    public async Task GetRelacionamentosMiniapps_DeveRetornarBadRequest_QuandoEntradaNula()
    {
        var result = await _controller.GetRelacionamentosMiniapps(null!);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("obrigat", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetRelacionamentosMiniapps_DeveRetornarBadRequest_QuandoGuidVazio()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto { CoMiniapp = Guid.Empty };

        // Act
        var result = await _controller.GetRelacionamentosMiniapps(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("vazio", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetRelacionamentosMiniapps_DeveRetornarNotFound_QuandoBusinessException()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto { CoMiniapp = Guid.NewGuid() };
        var erro = new ErroDto { Codigo = "404", Mensagem = "Não encontrado" };
        _serviceMock.Setup(s => s.ListarRelacionamentosAsync(entrada)).ThrowsAsync(new BusinessException(erro));

        // Act
        var result = await _controller.GetRelacionamentosMiniapps(entrada);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(erro, notFound.Value);
    }

    [Fact]
    public async Task GetRelacionamentosMiniapps_DeveRetornarStatus500_QuandoExceptionGenerica()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto { CoMiniapp = Guid.NewGuid() };
        _serviceMock.Setup(s => s.ListarRelacionamentosAsync(entrada)).ThrowsAsync(new Exception("Erro interno"));

        // Act
        var result = await _controller.GetRelacionamentosMiniapps(entrada);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        Assert.Contains("Erro interno", statusResult.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PutRelacionamentoMiniapp_DeveRetornarNoContent_QuandoDadosValidos()
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = Guid.NewGuid(),
            CoMiniappFilhoOriginal = Guid.NewGuid(),
            CoMiniappPaiNovo = Guid.NewGuid(),
            CoMiniappFilhoNovo = Guid.NewGuid()
        };

        // Act
        var result = await _controller.PutRelacionamentoMiniapp(entrada);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.EditarRelacionamentoAsync(entrada), Times.Once);
    }

    [Fact]
    public async Task PutRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoEntradaNula()
    {
        var result = await _controller.PutRelacionamentoMiniapp(null!);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("obrigat", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PutRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoGuidsInvalidos()
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = Guid.Empty,
            CoMiniappFilhoOriginal = Guid.NewGuid(),
            CoMiniappPaiNovo = Guid.NewGuid(),
            CoMiniappFilhoNovo = Guid.NewGuid()
        };

        // Act
        var result = await _controller.PutRelacionamentoMiniapp(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task PutRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoSemMudancas()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = id,
            CoMiniappFilhoOriginal = id,
            CoMiniappPaiNovo = id,
            CoMiniappFilhoNovo = id
        };

        // Act
        var result = await _controller.PutRelacionamentoMiniapp(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task PutRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoAutorelacionamento()
    {
        // Arrange
        var guidPaiOriginal = Guid.NewGuid();
        var guidFilhoOriginal = Guid.NewGuid();
        var guidNovo = Guid.NewGuid();
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = guidPaiOriginal,
            CoMiniappFilhoOriginal = guidFilhoOriginal,
            CoMiniappPaiNovo = guidNovo,
            CoMiniappFilhoNovo = guidNovo // autorelacionamento
        };
        // Act
        var result = await _controller.PutRelacionamentoMiniapp(entrada);
        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("pai dele mesmo", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("404", typeof(NotFoundObjectResult))]
    [InlineData("409", typeof(ConflictObjectResult))]
    [InlineData("400", typeof(BadRequestObjectResult))]
    public async Task PutRelacionamentoMiniapp_ComBusinessExceptionDiferentesCodigos_DeveRetornarStatusCorreto(
        string codigoErro, Type tipoResultadoEsperado)
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = Guid.NewGuid(),
            CoMiniappFilhoOriginal = Guid.NewGuid(),
            CoMiniappPaiNovo = Guid.NewGuid(),
            CoMiniappFilhoNovo = Guid.NewGuid()
        };

        var businessException = new BusinessException("ERRO_TESTE", "Erro de teste", int.Parse(codigoErro));
        _serviceMock.Setup(s => s.EditarRelacionamentoAsync(entrada)).ThrowsAsync(businessException);

        // Act
        var resultado = await _controller.PutRelacionamentoMiniapp(entrada);

        // Assert
        Assert.IsType(tipoResultadoEsperado, resultado);
    }

    [Fact]
    public async Task DeleteRelacionamentoMiniapp_DeveRetornarNoContent_QuandoDadosValidos()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };

        // Act
        var result = await _controller.DeleteRelacionamentoMiniapp(entrada);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.ExcluirRelacionamentoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoEntradaNula()
    {
        var result = await _controller.DeleteRelacionamentoMiniapp(null!);
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("obrigat", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteRelacionamentoMiniapp_DeveRetornarBadRequest_QuandoGuidsVazios()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.Empty, CoMiniappFilho = Guid.Empty };

        // Act
        var result = await _controller.DeleteRelacionamentoMiniapp(entrada);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Theory]
    [InlineData("404", typeof(NotFoundObjectResult))]
    [InlineData("400", typeof(BadRequestObjectResult))]
    public async Task DeleteRelacionamentoMiniapp_ComBusinessExceptionDiferentesCodigos_DeveRetornarStatusCorreto(
        string codigoErro, Type tipoResultadoEsperado)
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };

        var businessException = new BusinessException("ERRO_TESTE", "Erro de teste", int.Parse(codigoErro));
        _serviceMock.Setup(s => s.ExcluirRelacionamentoAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(businessException);

        // Act
        var result = await _controller.DeleteRelacionamentoMiniapp(entrada);

        // Assert
        Assert.IsType(tipoResultadoEsperado, result);
    }

    [Fact]
    public async Task DeleteRelacionamentoMiniapp_DeveRetornarStatus500_QuandoExceptionGenerica()
    {
        // Arrange
        var entrada = new IncluirRelacionamentoMiniappDto { CoMiniappPai = Guid.NewGuid(), CoMiniappFilho = Guid.NewGuid() };
        _serviceMock.Setup(s => s.ExcluirRelacionamentoAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new Exception("Erro interno"));

        // Act
        var result = await _controller.DeleteRelacionamentoMiniapp(entrada);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        Assert.Contains("Erro interno", statusResult.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
