using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using API_Miniapp_Gestao.Services;
using API_Miniapp_Gestao.Services.Interfaces;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Tests.Services
{
    public class MiniappVersaoServiceTests
    {
        private readonly Mock<IMiniAppVersaoRepository> _mockRepositorio;
        private readonly Mock<ILogger<MiniappVersaoService>> _mockLogger;
        private readonly MiniappVersaoService _servico;

        public MiniappVersaoServiceTests()
        {
            _mockRepositorio = new Mock<IMiniAppVersaoRepository>();
            _mockLogger = new Mock<ILogger<MiniappVersaoService>>();
            _servico = new MiniappVersaoService(_mockRepositorio.Object, _mockLogger.Object);
        }

        #region Testes CriarVersaoMiniappAsync

        [Fact]
        public async Task CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var coVersao = Guid.NewGuid();

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 2.0m,
                PcExpansao = 15.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "2.0.1"
            };

            var retornoEsperado = new RetornoCriarVersaoMiniappDto
            {
                CoVersao = coVersao,
                CoMiniapp = coMiniapp,
                NuVersao = 2.0m,
                PcExpansao = 15.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "2.0.1"
            };

            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _servico.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(retornoEsperado.CoVersao, resultado.CoVersao);
            Assert.Equal(retornoEsperado.CoMiniapp, resultado.CoMiniapp);
            Assert.Equal(retornoEsperado.NuVersao, resultado.NuVersao);
            Assert.Equal(retornoEsperado.PcExpansao, resultado.PcExpansao);
            Assert.Equal(retornoEsperado.IcAtivo, resultado.IcAtivo);
            Assert.Equal(retornoEsperado.EdVersaoMiniapp, resultado.EdVersaoMiniapp);

            _mockRepositorio.Verify(r => r.CriarVersaoMiniappAsync(entradaCriarVersao), Times.Once);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_QuandoRepositorioLancaKeyNotFoundException_DeveRepassarExcecao()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var mensagemErro = "Miniapp não encontrado";
            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(new KeyNotFoundException(mensagemErro));

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.CriarVersaoMiniappAsync(entradaCriarVersao)
            );

            Assert.IsType<KeyNotFoundException>(excecao.InnerException);
            Assert.Equal(mensagemErro, excecao.InnerException.Message);
            _mockRepositorio.Verify(r => r.CriarVersaoMiniappAsync(entradaCriarVersao), Times.Once);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_QuandoRepositorioLancaException_DeveRepassarExcecao()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var mensagemErro = "Erro interno no banco de dados";
            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.CriarVersaoMiniappAsync(entradaCriarVersao)
            );

            Assert.IsType<Exception>(excecao.InnerException);
            Assert.Equal(mensagemErro, excecao.InnerException.Message);
            _mockRepositorio.Verify(r => r.CriarVersaoMiniappAsync(entradaCriarVersao), Times.Once);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_DeveLogOperacoes()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var retornoEsperado = new RetornoCriarVersaoMiniappDto
            {
                CoVersao = Guid.NewGuid(),
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                PcExpansao = 0.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "1.0"
            };

            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            await _servico.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando criação de nova versão")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Versão do miniapp criada com sucesso")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_QuandoOcorreErro_DeveLogErro()
        {
            // Arrange
            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var excecaoEsperada = new Exception("Erro simulado");
            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ThrowsAsync(excecaoEsperada);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.CriarVersaoMiniappAsync(entradaCriarVersao)
            );

            Assert.IsType<Exception>(excecao.InnerException);
            Assert.Equal("Erro simulado", excecao.InnerException.Message);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro ao criar versão do miniapp")),
                    excecaoEsperada,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("1.5", null, false)]
        [InlineData(null, "5.0", true)]
        [InlineData(null, null, true)]
        public async Task CriarVersaoMiniappAsync_ComValoresOpcionaisNulos_DeveProcessarCorretamente(
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

            _mockRepositorio
                .Setup(r => r.CriarVersaoMiniappAsync(It.IsAny<EntradaCriarVersaoMiniappDto>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _servico.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(retornoEsperado.NuVersao, resultado.NuVersao);
            Assert.Equal(retornoEsperado.PcExpansao, resultado.PcExpansao);
            Assert.Equal(retornoEsperado.IcAtivo, resultado.IcAtivo);
        }

        #endregion

        #region Testes ListarVersoesMiniappAsync

        [Fact]
        public async Task ListarVersoesMiniappAsync_ComEntradaValida_DeveRetornarListaVersoes()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniapp.ToString()
            };

            var versoesEsperadas = new List<VersaoMiniappDto>
            {
                new VersaoMiniappDto
                {
                    CoVersao = Guid.NewGuid(),
                    NuVersao = 1.0m,
                    PcExpansao = 0.0m,
                    IcAtivo = true,
                    EdVersaoMiniapp = "https://releases.example.com/v1.0"
                },
                new VersaoMiniappDto
                {
                    CoVersao = Guid.NewGuid(),
                    NuVersao = 2.0m,
                    PcExpansao = 10.0m,
                    IcAtivo = false,
                    EdVersaoMiniapp = "https://releases.example.com/v2.0"
                }
            };

            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ReturnsAsync(versoesEsperadas);

            // Act
            var resultado = await _servico.ListarVersoesMiniappAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.RetornoListaVersoes.Count);
            Assert.Equal(versoesEsperadas[0].CoVersao, resultado.RetornoListaVersoes[0].CoVersao);
            Assert.Equal(versoesEsperadas[1].CoVersao, resultado.RetornoListaVersoes[1].CoVersao);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_ComCoMiniappNulo_DeveLancarArgumentException()
        {
            // Arrange
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = null
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.Contains("CoMiniapp é obrigatório", excecao.Message);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_ComCoMiniappVazio_DeveLancarArgumentException()
        {
            // Arrange
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = string.Empty
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.Contains("CoMiniapp é obrigatório", excecao.Message);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_ComCoMiniappInvalido_DeveLancarArgumentException()
        {
            // Arrange
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = "guid-invalido"
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.Contains("CoMiniapp deve ser um GUID válido", excecao.Message);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_QuandoRepositorioLancaKeyNotFoundException_DeveRepassarExcecao()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniapp.ToString()
            };

            var excecaoEsperada = new KeyNotFoundException("Miniapp não encontrado");
            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ThrowsAsync(excecaoEsperada);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.IsType<KeyNotFoundException>(excecao.InnerException);
            Assert.Equal(excecaoEsperada.Message, excecao.InnerException.Message);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_QuandoRepositorioLancaException_DeveRepassarExcecao()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniapp.ToString()
            };

            var excecaoEsperada = new Exception("Erro interno");
            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ThrowsAsync(excecaoEsperada);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.IsType<Exception>(excecao.InnerException);
            Assert.Equal(excecaoEsperada.Message, excecao.InnerException.Message);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_DeveLogOperacoes()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniapp.ToString()
            };

            var versoes = new List<VersaoMiniappDto>();
            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ReturnsAsync(versoes);

            // Act
            await _servico.ListarVersoesMiniappAsync(entrada);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando listagem de versões")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Listagem de versões concluída")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ListarVersoesMiniappAsync_QuandoOcorreErro_DeveLogErro()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniapp.ToString()
            };

            var excecaoEsperada = new Exception("Erro simulado");
            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ThrowsAsync(excecaoEsperada);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _servico.ListarVersoesMiniappAsync(entrada)
            );

            Assert.IsType<Exception>(excecao.InnerException);
            Assert.Equal("Erro simulado", excecao.InnerException.Message);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro ao listar versões do miniapp")),
                    excecaoEsperada,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task ListarVersoesMiniappAsync_ComDiferentesGUIDs_DeveProcessarCorretamente(string coMiniappStr)
        {
            // Arrange
            var coMiniapp = Guid.Parse(coMiniappStr);
            var entrada = new EntradaMiniappDto
            {
                CoMiniapp = coMiniappStr
            };

            var versoes = new List<VersaoMiniappDto>
            {
                new VersaoMiniappDto
                {
                    CoVersao = Guid.NewGuid(),
                    NuVersao = 1.0m,
                    PcExpansao = 0.0m,
                    IcAtivo = true
                }
            };

            _mockRepositorio
                .Setup(r => r.GetVersoesByMiniappAsync(coMiniapp))
                .ReturnsAsync(versoes);

            // Act
            var resultado = await _servico.ListarVersoesMiniappAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado.RetornoListaVersoes);
            Assert.Equal(versoes[0].CoVersao, resultado.RetornoListaVersoes[0].CoVersao);
        }

        #endregion
    }
}
