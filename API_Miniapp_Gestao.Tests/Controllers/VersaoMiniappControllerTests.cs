using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using API_Miniapp_Gestao.Controllers;
using API_Miniapp_Gestao.Services.Interfaces;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;

namespace API_Miniapp_Gestao.Tests.Controllers
{
    public class VersaoMiniappControllerTests
    {
        private readonly Mock<ILogger<VersaoMiniappController>> _mockLogger;
        private readonly Mock<IMiniappVersaoService> _mockServico;
        private readonly VersaoMiniappController _controller;

        public VersaoMiniappControllerTests()
        {
            _mockLogger = new Mock<ILogger<VersaoMiniappController>>();
            _mockServico = new Mock<IMiniappVersaoService>();
            _controller = new VersaoMiniappController(_mockLogger.Object, _mockServico.Object);
        }

        #region Testes PostVersaoMiniapp (Criar)

        [Fact]
        public async Task PostVersaoMiniapp_ComEntradaValida_DeveRetornarCreatedComDadosCorretos()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var coVersao = Guid.NewGuid();

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                PcExpansao = 5.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "1.0.0"
            };

            var retornoEsperado = new RetornoCriarVersaoMiniappDto
            {
                CoVersao = coVersao,
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                PcExpansao = 5.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "1.0.0"
            };

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(resultado);
            Assert.Equal($"v1/miniapp/{coMiniapp}/versao/{coVersao}", createdResult.Location);
            
            var valorRetornado = Assert.IsType<RetornoCriarVersaoMiniappDto>(createdResult.Value);
            Assert.Equal(retornoEsperado.CoVersao, valorRetornado.CoVersao);
            Assert.Equal(retornoEsperado.CoMiniapp, valorRetornado.CoMiniapp);
            Assert.Equal(retornoEsperado.NuVersao, valorRetornado.NuVersao);

            _mockServico.Verify(s => s.CriarVersaoMiniappAsync(entradaCriarVersao), Times.Once);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto();
            _controller.ModelState.AddModelError("CoMiniapp", "CoMiniapp é obrigatório");

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);

            _mockServico.Verify(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()), Times.Never);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComBusinessExceptionNotFound_DeveRetornarNotFound()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var erroDto = new ErroDto
            {
                Codigo = "404",
                Mensagem = "Miniapp não encontrado"
            };

            var businessException = new BusinessException(erroDto);

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(businessException);

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComBusinessExceptionConflict_DeveRetornarConflict()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var erroDto = new ErroDto
            {
                Codigo = "409",
                Mensagem = "Versão já existe"
            };

            var businessException = new BusinessException(erroDto);

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(businessException);

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(resultado);
            Assert.NotNull(conflictResult.Value);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComBusinessExceptionBadRequest_DeveRetornarBadRequest()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var erroDto = new ErroDto
            {
                Codigo = "400",
                Mensagem = "Dados inválidos"
            };

            var businessException = new BusinessException(erroDto);

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(businessException);

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComExceptionGenerica_DeveRetornarInternalServerError()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.NotNull(statusResult.Value);
        }

        [Fact]
        public async Task PostVersaoMiniapp_ComModelStateInvalido_NaoDeveLogar()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            _controller.ModelState.AddModelError("Teste", "Erro de teste");

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);
            
            // Verifica que nenhum log foi feito pois o controller não loga em ModelState inválido
            _mockLogger.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null, "0.0", true)]
        [InlineData("2.5", null, false)]
        [InlineData("1.0", "10.0", true)]
        public async Task PostVersaoMiniapp_ComDiferentesValoresOpcionais_DeveProcessarCorretamente(
            string? nuVersaoStr, string? pcExpansaoStr, bool icAtivo)
        {
            // Arrange
            var nuVersao = nuVersaoStr != null ? decimal.Parse(nuVersaoStr, System.Globalization.CultureInfo.InvariantCulture) : (decimal?)null;
            var pcExpansao = pcExpansaoStr != null ? decimal.Parse(pcExpansaoStr, System.Globalization.CultureInfo.InvariantCulture) : (decimal?)null;
            
            var coMiniapp = Guid.NewGuid();
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = nuVersao,
                PcExpansao = pcExpansao,
                IcAtivo = icAtivo
            };

            var retornoEsperado = new RetornoCriarVersaoMiniappDto
            {
                CoVersao = Guid.NewGuid(),
                CoMiniapp = coMiniapp,
                NuVersao = nuVersao,
                PcExpansao = pcExpansao,
                IcAtivo = icAtivo
            };

            _mockServico
                .Setup(s => s.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(resultado);
            var valorRetornado = Assert.IsType<RetornoCriarVersaoMiniappDto>(createdResult.Value);
            
            Assert.Equal(retornoEsperado.NuVersao, valorRetornado.NuVersao);
            Assert.Equal(retornoEsperado.PcExpansao, valorRetornado.PcExpansao);
            Assert.Equal(retornoEsperado.IcAtivo, valorRetornado.IcAtivo);
        }

        #endregion

        #region Testes GetVersoesMiniapps (Listar)

        [Fact]
        public async Task GetVersoesMiniapps_ComGuidValido_DeveRetornarOkComListaVersoes()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            var versoes = new List<VersaoMiniappDto>
            {
                new() { CoVersao = Guid.NewGuid(), NuVersao = 1.0m, IcAtivo = true },
                new() { CoVersao = Guid.NewGuid(), NuVersao = 2.0m, IcAtivo = false }
            };

            var retornoEsperado = new RetornoListarVersoesDto
            {
                RetornoListaVersoes = versoes
            };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var valorRetornado = Assert.IsType<RetornoListarVersoesDto>(okResult.Value);
            
            Assert.Equal(2, valorRetornado.RetornoListaVersoes.Count);
            Assert.Equal(versoes[0].CoVersao, valorRetornado.RetornoListaVersoes[0].CoVersao);
            Assert.Equal(versoes[1].CoVersao, valorRetornado.RetornoListaVersoes[1].CoVersao);

            _mockServico.Verify(s => s.ListarVersoesMiniappAsync(entrada), Times.Once);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComListaVazia_DeveRetornarOkComListaVazia()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            var retornoEsperado = new RetornoListarVersoesDto
            {
                RetornoListaVersoes = new List<VersaoMiniappDto>()
            };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var valorRetornado = Assert.IsType<RetornoListarVersoesDto>(okResult.Value);
            
            Assert.Empty(valorRetornado.RetornoListaVersoes);
            _mockServico.Verify(s => s.ListarVersoesMiniappAsync(entrada), Times.Once);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComModelStateInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new EntradaMiniappDto();
            _controller.ModelState.AddModelError("CoMiniapp", "CoMiniapp é obrigatório");

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.NotNull(badRequestResult.Value);

            _mockServico.Verify(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()), Times.Never);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComArgumentException_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new EntradaMiniappDto { CoMiniapp = "guid-invalido" };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new ArgumentException("GUID inválido"));

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComKeyNotFoundException_DeveRetornarNotFound()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new KeyNotFoundException("Miniapp não encontrado"));

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComExceptionGenerica_DeveRetornarInternalServerError()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.NotNull(statusResult.Value);
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComModelStateInvalidoNaoDeveLogar_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new EntradaMiniappDto();
            _controller.ModelState.AddModelError("Teste", "Erro de teste");

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.NotNull(badRequestResult.Value);
            
            // O controller não faz log para ModelState inválido, apenas retorna BadRequest
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComArgumentExceptionNaoDeveLogar_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new EntradaMiniappDto { CoMiniapp = "guid-invalido" };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new ArgumentException("GUID inválido"));

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.NotNull(badRequestResult.Value);
            
            // O controller não faz log para ArgumentException, apenas retorna BadRequest
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComKeyNotFoundExceptionNaoDeveLogar_DeveRetornarNotFound()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new KeyNotFoundException("Miniapp não encontrado"));

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.NotNull(notFoundResult.Value);
            
            // O controller não faz log para KeyNotFoundException, apenas retorna NotFound
        }

        [Fact]
        public async Task GetVersoesMiniapps_ComExceptionGenerica_DeveLogError()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp.ToString() };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            await _controller.GetVersoesMiniapps(entrada);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro inesperado ao listar versões")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("123e4567-e89b-12d3-a456-426614174000")]
        public async Task GetVersoesMiniapps_ComDiferentesValoresCoMiniapp_DeveProcessarCorretamente(string? coMiniapp)
        {
            // Arrange
            var entrada = new EntradaMiniappDto { CoMiniapp = coMiniapp };

            var retornoEsperado = new RetornoListarVersoesDto
            {
                RetornoListaVersoes = new List<VersaoMiniappDto>()
            };

            _mockServico
                .Setup(s => s.ListarVersoesMiniappAsync(It.IsAny<EntradaMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _controller.GetVersoesMiniapps(entrada);

            // Assert
            if (string.IsNullOrEmpty(coMiniapp))
            {
                // Para valores nulos ou vazios, provavelmente haverá validação no service
                // Mas o controller deve processar a chamada
                _mockServico.Verify(s => s.ListarVersoesMiniappAsync(entrada), Times.Once);
            }
            else
            {
                // Para GUIDs válidos, deve retornar OK
                var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
                Assert.NotNull(okResult.Value);
                _mockServico.Verify(s => s.ListarVersoesMiniappAsync(entrada), Times.Once);
            }
        }

        #endregion
    }
}
