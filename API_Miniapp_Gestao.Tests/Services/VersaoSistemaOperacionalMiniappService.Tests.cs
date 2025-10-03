using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services;

namespace API_Miniapp_Gestao.Tests.Services
{
    public class VersaoSistemaOperacionalMiniappServiceTests
    {
        private readonly Mock<IVersaoSistemaOperacionalMiniappRepository> _mockRepository;
        private readonly Mock<ILogger<VersaoSistemaOperacionalMiniappService>> _mockLogger;
        private readonly VersaoSistemaOperacionalMiniappService _service;

        public VersaoSistemaOperacionalMiniappServiceTests()
        {
            _mockRepository = new Mock<IVersaoSistemaOperacionalMiniappRepository>();
            _mockLogger = new Mock<ILogger<VersaoSistemaOperacionalMiniappService>>();
            _service = new VersaoSistemaOperacionalMiniappService(_mockRepository.Object, _mockLogger.Object);
        }

        #region CriarVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task CriarVersaoSistemaOperacionalAsync_ComDadosValidos_DeveRetornarSucesso()
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
                CoPlataforma = entrada.CoPlataforma,
                NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional,
                NuVersaoSdk = entrada.NuVersaoSdk
            };

            _mockRepository
                .Setup(r => r.CriarVersaoSistemaOperacionalAsync(entrada))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _service.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(resultadoEsperado.CoVersaoSistemaOperacional, resultado.CoVersaoSistemaOperacional);
            Assert.Equal(resultadoEsperado.CoPlataforma, resultado.CoPlataforma);
            Assert.Equal(resultadoEsperado.NuVersaoSistemaOperacional, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(resultadoEsperado.NuVersaoSdk, resultado.NuVersaoSdk);

            _mockRepository.Verify(r => r.CriarVersaoSistemaOperacionalAsync(entrada), Times.Once);
        }

        [Fact]
        public async Task CriarVersaoSistemaOperacionalAsync_ComEntradaNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CriarVersaoSistemaOperacionalAsync(null!));
        }

        #endregion

        #region ListarVersoesSistemaOperacionalAsync Tests

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_ComVersoes_DeveRetornarLista()
        {
            // Arrange
            var entrada = new ListaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A"
            };

            var versoes = new List<ListaRetornoVersaoSistemaOperacionalMiniappDto>
            {
                new ListaRetornoVersaoSistemaOperacionalMiniappDto
                {
                    CoVersaoSistemaOperacional = Guid.NewGuid(),
                    CoPlataforma = "A",
                    NuVersaoSistemaOperacional = 12.0m,
                    NuVersaoSdk = 31.0m
                },
                new ListaRetornoVersaoSistemaOperacionalMiniappDto
                {
                    CoVersaoSistemaOperacional = Guid.NewGuid(),
                    CoPlataforma = "A",
                    NuVersaoSistemaOperacional = 11.0m,
                    NuVersaoSdk = 30.0m
                }
            };

            _mockRepository
                .Setup(r => r.ListarVersoesSistemaOperacionalAsync(entrada))
                .ReturnsAsync(versoes);

            // Act
            var resultado = await _service.ListarVersoesSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, v => v.NuVersaoSistemaOperacional == 12.0m);
            Assert.Contains(resultado, v => v.NuVersaoSistemaOperacional == 11.0m);

            _mockRepository.Verify(r => r.ListarVersoesSistemaOperacionalAsync(entrada), Times.Once);
        }

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_SemVersoes_DeveLancarBusinessException()
        {
            // Arrange
            var entrada = new ListaVersaoSistemaOperacionalMiniappDto();
            var versoes = new List<ListaRetornoVersaoSistemaOperacionalMiniappDto>();

            _mockRepository
                .Setup(r => r.ListarVersoesSistemaOperacionalAsync(entrada))
                .ReturnsAsync(versoes);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ListarVersoesSistemaOperacionalAsync(entrada));
            Assert.Equal("NENHUMA_VERSAO_ENCONTRADA", exception.Erro?.Codigo);
            Assert.Equal(404, exception.Erro?.StatusCode);
        }

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_ComListaNula_DeveLancarBusinessException()
        {
            // Arrange
            var entrada = new ListaVersaoSistemaOperacionalMiniappDto();

            _mockRepository
                .Setup(r => r.ListarVersoesSistemaOperacionalAsync(entrada))
                .ReturnsAsync((List<ListaRetornoVersaoSistemaOperacionalMiniappDto>)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ListarVersoesSistemaOperacionalAsync(entrada));
            Assert.Equal("NENHUMA_VERSAO_ENCONTRADA", exception.Erro?.Codigo);
        }

        #endregion

        #region AtualizarVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task AtualizarVersaoSistemaOperacionalAsync_ComDadosValidos_DeveRetornarSucesso()
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

            _mockRepository
                .Setup(r => r.AtualizarVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional, entrada))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _service.AtualizarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(entrada.CoVersaoSistemaOperacional, resultado.CoVersaoSistemaOperacional);
            Assert.Equal(entrada.CoPlataforma, resultado.CoPlataforma);
            Assert.Equal(entrada.NuVersaoSistemaOperacional, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSdk, resultado.NuVersaoSdk);

            _mockRepository.Verify(r => r.AtualizarVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional, entrada), Times.Once);
        }

        [Fact]
        public async Task AtualizarVersaoSistemaOperacionalAsync_ComEntradaNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AtualizarVersaoSistemaOperacionalAsync(null!));
        }

        #endregion

        #region ExcluirVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComVersaoExistente_DeveExcluirComSucesso()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };

            _mockRepository
                .Setup(r => r.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.ExcluirVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ExcluirVersaoSistemaOperacionalAsync(entrada);

            // Assert
            _mockRepository.Verify(r => r.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional), Times.Once);
            _mockRepository.Verify(r => r.ExcluirVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional), Times.Once);
        }

        [Fact]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComVersaoInexistente_DeveLancarBusinessException()
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid()
            };

            _mockRepository
                .Setup(r => r.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ExcluirVersaoSistemaOperacionalAsync(entrada));
            Assert.Equal("VERSAO_NAO_ENCONTRADA", exception.Erro?.Codigo);
            Assert.Equal(404, exception.Erro?.StatusCode);
            Assert.Contains(entrada.CoVersaoSistemaOperacional.ToString(), exception.Erro?.Mensagem ?? "");

            _mockRepository.Verify(r => r.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional), Times.Once);
            _mockRepository.Verify(r => r.ExcluirVersaoSistemaOperacionalAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComEntradaNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ExcluirVersaoSistemaOperacionalAsync(null!));
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComGuidVazio_DeveLancarBusinessException(string guidString)
        {
            // Arrange
            var entrada = new RemoveVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.Parse(guidString)
            };

            _mockRepository
                .Setup(r => r.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ExcluirVersaoSistemaOperacionalAsync(entrada));
            Assert.Equal("VERSAO_NAO_ENCONTRADA", exception.Erro?.Codigo);
        }

        #endregion

        #region Testes de Integração Básicos

        [Fact]
        public async Task CriarVersaoSistemaOperacionalAsync_DeveGerarGuidUnico()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 11.0m,
                NuVersaoSdk = 30.0m
            };

            var resultado1 = new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = entrada.CoPlataforma,
                NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional,
                NuVersaoSdk = entrada.NuVersaoSdk
            };

            var resultado2 = new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = entrada.CoPlataforma,
                NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional,
                NuVersaoSdk = entrada.NuVersaoSdk
            };

            _mockRepository
                .SetupSequence(r => r.CriarVersaoSistemaOperacionalAsync(entrada))
                .ReturnsAsync(resultado1)
                .ReturnsAsync(resultado2);

            // Act
            var primeiraVersao = await _service.CriarVersaoSistemaOperacionalAsync(entrada);
            var segundaVersao = await _service.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotEqual(primeiraVersao.CoVersaoSistemaOperacional, segundaVersao.CoVersaoSistemaOperacional);
        }

        [Theory]
        [InlineData("A", 12.0, 31.0)]
        [InlineData("I", 15.0, 15.2)]
        public async Task CriarVersaoSistemaOperacionalAsync_ComDiferentesDados_DeveProcessarCorretamente(
            string coPlataforma, decimal nuVersaoSO, decimal nuVersaoSdk)
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = coPlataforma,
                NuVersaoSistemaOperacional = nuVersaoSO,
                NuVersaoSdk = nuVersaoSdk
            };

            var resultadoEsperado = new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = coPlataforma,
                NuVersaoSistemaOperacional = nuVersaoSO,
                NuVersaoSdk = nuVersaoSdk
            };

            _mockRepository
                .Setup(r => r.CriarVersaoSistemaOperacionalAsync(entrada))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _service.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.Equal(coPlataforma, resultado.CoPlataforma);
            Assert.Equal(nuVersaoSO, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(nuVersaoSdk, resultado.NuVersaoSdk);
        }

        #endregion
    }
}
