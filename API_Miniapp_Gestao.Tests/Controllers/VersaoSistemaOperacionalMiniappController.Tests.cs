using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using API_Miniapp_Gestao.Controllers;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Services.Interfaces;

namespace API_Miniapp_Gestao.Tests.Controllers
{
    public class VersaoSistemaOperacionalMiniappControllerTests
    {
        private readonly Mock<ILogger<VersaoSistemaOperacionalMiniappController>> _mockLogger;
        private readonly Mock<IVersaoSistemaOperacionalMiniappService> _mockService;
        private readonly VersaoSistemaOperacionalMiniappController _controller;

        public VersaoSistemaOperacionalMiniappControllerTests()
        {
            _mockLogger = new Mock<ILogger<VersaoSistemaOperacionalMiniappController>>();
            _mockService = new Mock<IVersaoSistemaOperacionalMiniappService>();
            _controller = new VersaoSistemaOperacionalMiniappController(_mockLogger.Object, _mockService.Object);
        }

        #region PostVersaoSistemaOperacionalMiniapp Tests

        [Fact]
        public async Task PostVersaoSistemaOperacionalMiniapp_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            var resultadoEsperado = new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _mockService
                .Setup(s => s.CriarVersaoSistemaOperacionalAsync(entrada))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.PostVersaoSistemaOperacionalMiniapp(entrada);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(resultado);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal($"v1/sistema-operacional/{resultadoEsperado.CoVersaoSistemaOperacional}", createdResult.Location);
            Assert.Equal(resultadoEsperado, createdResult.Value);
        }

        [Fact]
        public async Task PostVersaoSistemaOperacionalMiniapp_ComArgumentException_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };
            var exceptionMessage = "Argumentos inválidos";

            _mockService
                .Setup(s => s.CriarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new ArgumentException(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.PostVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task PostVersaoSistemaOperacionalMiniapp_ComBusinessException_DeveRetornarStatusCodeCorreto()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };
            var businessException = new BusinessException("CODIGO_ERRO", "Mensagem de erro", 400);

            _mockService
                .Setup(s => s.CriarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(businessException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _controller.PostVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal("CODIGO_ERRO", exception.Erro.Codigo);
            Assert.Equal("Mensagem de erro", exception.Erro.Mensagem);
            Assert.Equal(400, exception.Erro.StatusCode);
        }

        [Fact]
        public async Task PostVersaoSistemaOperacionalMiniapp_ComExcecaoGenerica_DeveRetornarStatusCode500()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _mockService
                .Setup(s => s.CriarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new Exception("Erro genérico"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.PostVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal("Erro genérico", exception.Message);
        }

        #endregion

        #region GetVersaoSistemaOperacionalMiniapp Tests

        [Fact]
        public async Task GetVersaoSistemaOperacionalMiniapp_ComSucesso_DeveRetornarOk()
        {
            // Arrange
            var entrada = new ListaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A"
            };

            var resultadoEsperado = new List<ListaRetornoVersaoSistemaOperacionalMiniappDto>
            {
                new ListaRetornoVersaoSistemaOperacionalMiniappDto
                {
                    CoVersaoSistemaOperacional = Guid.NewGuid(),
                    CoPlataforma = "A",
                    NuVersaoSistemaOperacional = 12.0m,
                    NuVersaoSdk = 31.0m
                }
            };

            _mockService
                .Setup(s => s.ListarVersoesSistemaOperacionalAsync(It.IsAny<ListaVersaoSistemaOperacionalMiniappDto>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.GetVersaoSistemaOperacionalMiniapp(entrada);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task GetVersaoSistemaOperacionalMiniapp_ComEntradaNula_DeveUsarEntradaVazia()
        {
            // Arrange
            var resultadoEsperado = new List<ListaRetornoVersaoSistemaOperacionalMiniappDto>();

            _mockService
                .Setup(s => s.ListarVersoesSistemaOperacionalAsync(It.IsAny<ListaVersaoSistemaOperacionalMiniappDto>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.GetVersaoSistemaOperacionalMiniapp(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
            _mockService.Verify(s => s.ListarVersoesSistemaOperacionalAsync(It.IsAny<ListaVersaoSistemaOperacionalMiniappDto>()), Times.Once);
        }

        #endregion

        #region PatchVersaoSistemaOperacionalMiniapp Tests

        [Fact]
        public async Task PatchVersaoSistemaOperacionalMiniapp_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };

            var resultadoEsperado = new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = entrada.CoVersaoSistemaOperacional,
                CoPlataforma = entrada.CoPlataforma,
                NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional,
                NuVersaoSdk = entrada.NuVersaoSdk
            };

            _mockService
                .Setup(s => s.AtualizarVersaoSistemaOperacionalAsync(entrada))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _controller.PatchVersaoSistemaOperacionalMiniapp(entrada);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(resultadoEsperado, okResult.Value);
        }

        [Fact]
        public async Task PatchVersaoSistemaOperacionalMiniapp_ComArgumentException_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };
            var exceptionMessage = "Dados inválidos";

            _mockService
                .Setup(s => s.AtualizarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new ArgumentException(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.PatchVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task PatchVersaoSistemaOperacionalMiniapp_ComKeyNotFoundException_DeveRetornarNotFound()
        {
            // Arrange
            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };
            var exceptionMessage = "Versão não encontrada";

            _mockService
                .Setup(s => s.AtualizarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new KeyNotFoundException(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.PatchVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task PatchVersaoSistemaOperacionalMiniapp_ComInvalidOperationException_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };
            var exceptionMessage = "Operação inválida";

            _mockService
                .Setup(s => s.AtualizarVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.PatchVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        #endregion

        #region DeleteVersaoSistemaOperacionalMiniapp Tests

        [Fact]
        public async Task DeleteVersaoSistemaOperacionalMiniapp_ComSucesso_DeveRetornarOk()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };

            _mockService
                .Setup(s => s.ExcluirVersaoSistemaOperacionalAsync(entrada))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.DeleteVersaoSistemaOperacionalMiniapp(entrada);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteVersaoSistemaOperacionalMiniapp_ComArgumentException_DeveRetornarBadRequest()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };
            var exceptionMessage = "ID inválido";

            _mockService
                .Setup(s => s.ExcluirVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new ArgumentException(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task DeleteVersaoSistemaOperacionalMiniapp_ComBusinessException_DeveRetornarStatusCodeCorreto()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };
            var businessException = new BusinessException("VERSAO_NAO_ENCONTRADA", "Versão não encontrada", 404);

            _mockService
                .Setup(s => s.ExcluirVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(businessException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _controller.DeleteVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal("VERSAO_NAO_ENCONTRADA", exception.Erro.Codigo);
            Assert.Equal("Versão não encontrada", exception.Erro.Mensagem);
            Assert.Equal(404, exception.Erro.StatusCode);
        }

        [Fact]
        public async Task DeleteVersaoSistemaOperacionalMiniapp_ComExcecaoGenerica_DeveRetornarStatusCode500()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };

            _mockService
                .Setup(s => s.ExcluirVersaoSistemaOperacionalAsync(entrada))
                .ThrowsAsync(new Exception("Erro genérico"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.DeleteVersaoSistemaOperacionalMiniapp(entrada));
            Assert.Equal("Erro genérico", exception.Message);
        }

        #endregion
    }
}
