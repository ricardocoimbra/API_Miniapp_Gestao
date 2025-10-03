using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using API_Miniapp_Gestao.Repositories;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Tests.Repositories
{
    public class MiniAppVersaoRepositoryTests : IDisposable
    {
        private readonly DbEscrita _dbEscrita;
        private readonly DbLeitura _dbLeitura;
        private readonly Mock<ILogger<MiniAppVersaoRepository>> _mockLogger;
        private readonly MiniAppVersaoRepository _repositorio;

        public MiniAppVersaoRepositoryTests()
        {
            // Configurar banco em memória para testes
            var optionsEscrita = new DbContextOptionsBuilder<DbEscrita>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var optionsLeitura = new DbContextOptionsBuilder<DbLeitura>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbEscrita = new DbEscrita(optionsEscrita);
            _dbLeitura = new DbLeitura(optionsLeitura);
            _mockLogger = new Mock<ILogger<MiniAppVersaoRepository>>();
            
            _repositorio = new MiniAppVersaoRepository(_dbEscrita, _dbLeitura, _mockLogger.Object);
        }

        #region Testes CriarVersaoMiniappAsync

        [Fact]
        public async Task CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            await CriarMiniappParaTeste(coMiniapp);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.5m,
                PcExpansao = 10.0m,
                IcAtivo = true,
                EdVersaoMiniapp = "1.5.0"
            };

            // Act
            var resultado = await _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(coMiniapp, resultado.CoMiniapp);
            Assert.Equal(1.5m, resultado.NuVersao);
            Assert.Equal(10.0m, resultado.PcExpansao);
            Assert.True(resultado.IcAtivo);
            Assert.Equal("1.5.0", resultado.EdVersaoMiniapp);
            Assert.NotEqual(Guid.Empty, resultado.CoVersao);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_ComValoresNulos_DeveUsarValoresPadrao()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            await CriarMiniappParaTeste(coMiniapp);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = null,
                PcExpansao = null,
                IcAtivo = false,
                EdVersaoMiniapp = null
            };

            // Act
            var resultado = await _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(coMiniapp, resultado.CoMiniapp);
            Assert.Equal(1.0m, resultado.NuVersao);
            Assert.Equal(0.0m, resultado.PcExpansao);
            Assert.False(resultado.IcAtivo);
            Assert.Contains("v1", resultado.EdVersaoMiniapp);
            Assert.StartsWith("https://", resultado.EdVersaoMiniapp);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_ComMiniappInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var coMiniappInexistente = Guid.NewGuid();

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniappInexistente,
                NuVersao = 1.0m,
                PcExpansao = 5.0m,
                IcAtivo = true
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao)
            );

            Assert.Contains(coMiniappInexistente.ToString(), excecao.Message);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_DeveGerarCoVersaoUnico()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            await CriarMiniappParaTeste(coMiniapp);

            var entradaCriarVersao1 = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var entradaCriarVersao2 = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 2.0m,
                IcAtivo = true
            };

            // Act
            var resultado1 = await _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao1);
            var resultado2 = await _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao2);

            // Assert
            Assert.NotEqual(resultado1.CoVersao, resultado2.CoVersao);
            Assert.NotEqual(Guid.Empty, resultado1.CoVersao);
            Assert.NotEqual(Guid.Empty, resultado2.CoVersao);
        }

        [Fact]
        public async Task CriarVersaoMiniappAsync_DeveLogOperacoes()
        {
            // Arrange
            var coMiniapp = Guid.NewGuid();
            await CriarMiniappParaTeste(coMiniapp);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                IcAtivo = true
            };

            // Act
            await _repositorio.CriarVersaoMiniappAsync(entradaCriarVersao);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando criação de versão")),
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

        #endregion

        #region Métodos Auxiliares

        private async Task CriarMiniappParaTeste(Guid coMiniapp)
        {
            var miniapp = new Nbmtb004Miniapp
            {
                CoMiniapp = coMiniapp,
                NoMiniapp = "Miniapp Teste",
                NoApelidoMiniapp = "Teste",
                DeMiniapp = "Descrição do teste",
                IcMiniappInicial = false,
                IcAtivo = true
            };

            // Adicionar nos dois contextos
            _dbEscrita.Nbmtb004Miniapps.Add(miniapp);
            await _dbEscrita.SaveChangesAsync();

            _dbLeitura.Nbmtb004Miniapps.Add(new Nbmtb004Miniapp
            {
                CoMiniapp = coMiniapp,
                NoMiniapp = "Miniapp Teste",
                NoApelidoMiniapp = "Teste",
                DeMiniapp = "Descrição do teste",
                IcMiniappInicial = false,
                IcAtivo = true
            });
            await _dbLeitura.SaveChangesAsync();
        }

        #endregion

        public void Dispose()
        {
            _dbEscrita?.Dispose();
            _dbLeitura?.Dispose();
        }
    }
}
