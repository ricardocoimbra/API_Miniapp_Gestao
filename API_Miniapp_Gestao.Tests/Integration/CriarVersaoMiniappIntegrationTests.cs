using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using API_Miniapp_Gestao.Tests.Base;
using API_Miniapp_Gestao.Repositories;
using API_Miniapp_Gestao.Services;
using API_Miniapp_Gestao.Controllers;
using API_Miniapp_Gestao.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API_Miniapp_Gestao.Tests.Integration
{
    public class CriarVersaoMiniappIntegrationTests : TestBase
    {
        private readonly Mock<ILogger<MiniAppVersaoRepository>> _mockLoggerRepo;
        private readonly Mock<ILogger<MiniappVersaoService>> _mockLoggerService;
        private readonly Mock<ILogger<VersaoMiniappController>> _mockLoggerController;

        public CriarVersaoMiniappIntegrationTests()
        {
            _mockLoggerRepo = new Mock<ILogger<MiniAppVersaoRepository>>();
            _mockLoggerService = new Mock<ILogger<MiniappVersaoService>>();
            _mockLoggerController = new Mock<ILogger<VersaoMiniappController>>();
        }

        [Fact]
        public async Task FluxoCompleto_CriarVersaoMiniapp_DeveProcessarComSucesso()
        {
            // Arrange
            var coMiniapp = await CriarMiniappParaTeste();

            var repositorio = new MiniAppVersaoRepository(DbEscrita, DbLeitura, _mockLoggerRepo.Object);
            var servico = new MiniappVersaoService(repositorio, _mockLoggerService.Object);
            var controller = new VersaoMiniappController(_mockLoggerController.Object, servico);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 2.5m,
                PcExpansao = 12.5m,
                IcAtivo = true,
                EdVersaoMiniapp = "2.5.0-beta"
            };

            // Act
            var resultadoController = await controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(resultadoController);
            var versaoCriada = Assert.IsType<RetornoCriarVersaoMiniappDto>(createdResult.Value);

            Assert.NotEqual(Guid.Empty, versaoCriada.CoVersao);
            Assert.Equal(coMiniapp, versaoCriada.CoMiniapp);
            Assert.Equal(2.5m, versaoCriada.NuVersao);
            Assert.Equal(12.5m, versaoCriada.PcExpansao);
            Assert.True(versaoCriada.IcAtivo);
            Assert.Equal("2.5.0-beta", versaoCriada.EdVersaoMiniapp);

            // Verificar se foi persistido no banco
            var versaoPersistida = DbLeitura.Nbmtb006VersaoMiniapps
                .FirstOrDefault(v => v.CoVersaoMiniapp == versaoCriada.CoVersao);

            Assert.NotNull(versaoPersistida);
            Assert.Equal(coMiniapp, versaoPersistida.CoMiniapp);
            Assert.Equal(2.5m, versaoPersistida.NuVersaoMiniapp);
            Assert.Equal(12.5m, versaoPersistida.PcExpansaoMiniapp);
            Assert.True(versaoPersistida.IcAtivo);
            Assert.Equal("2.5.0-beta", versaoPersistida.EdVersaoMiniapp);
        }

        [Fact]
        public async Task FluxoCompleto_CriarVersaoMiniappInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var coMiniappInexistente = Guid.NewGuid();

            var repositorio = new MiniAppVersaoRepository(DbEscrita, DbLeitura, _mockLoggerRepo.Object);
            var servico = new MiniappVersaoService(repositorio, _mockLoggerService.Object);
            var controller = new VersaoMiniappController(_mockLoggerController.Object, servico);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniappInexistente,
                NuVersao = 1.0m,
                IcAtivo = true
            };

            // Act
            var resultado = await controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);
            
            var valorRetornado = notFoundResult.Value;
            Assert.NotNull(valorRetornado);
        }

        [Fact]
        public async Task FluxoCompleto_CriarMultiplasVersoes_DeveGerarCodigosUnicos()
        {
            // Arrange
            var coMiniapp = await CriarMiniappParaTeste();

            var repositorio = new MiniAppVersaoRepository(DbEscrita, DbLeitura, _mockLoggerRepo.Object);
            var servico = new MiniappVersaoService(repositorio, _mockLoggerService.Object);
            var controller = new VersaoMiniappController(_mockLoggerController.Object, servico);

            var entrada1 = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 1.0m,
                IcAtivo = true
            };

            var entrada2 = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = 2.0m,
                IcAtivo = true
            };

            // Act
            var resultado1 = await controller.PostVersaoMiniapp(entrada1);
            var resultado2 = await controller.PostVersaoMiniapp(entrada2);

            // Assert
            var versao1 = Assert.IsType<RetornoCriarVersaoMiniappDto>(
                ((CreatedResult)resultado1).Value);
            var versao2 = Assert.IsType<RetornoCriarVersaoMiniappDto>(
                ((CreatedResult)resultado2).Value);

            Assert.NotEqual(versao1.CoVersao, versao2.CoVersao);
            Assert.Equal(coMiniapp, versao1.CoMiniapp);
            Assert.Equal(coMiniapp, versao2.CoMiniapp);
            Assert.Equal(1.0m, versao1.NuVersao);
            Assert.Equal(2.0m, versao2.NuVersao);

            // Verificar se ambas foram persistidas
            var versoesPersistidas = DbLeitura.Nbmtb006VersaoMiniapps
                .Where(v => v.CoMiniapp == coMiniapp)
                .ToList();

            Assert.Equal(2, versoesPersistidas.Count);
        }

        [Theory]
        [InlineData(null, null, true, "v1")]
        [InlineData("3.5", null, false, "v3.5")]
        [InlineData(null, "25.0", true, "v1")]
        [InlineData("4.2", "15.5", true, "v4.2")]
        public async Task FluxoCompleto_CriarVersaoComValoresOpcionais_DeveProcessarCorretamente(
            string? nuVersaoStr, string? pcExpansaoStr, bool icAtivo, string edVersaoEsperada)
        {
            // Arrange
            var nuVersao = nuVersaoStr != null ? decimal.Parse(nuVersaoStr, System.Globalization.CultureInfo.InvariantCulture) : (decimal?)null;
            var pcExpansao = pcExpansaoStr != null ? decimal.Parse(pcExpansaoStr, System.Globalization.CultureInfo.InvariantCulture) : (decimal?)null;
            
            var coMiniapp = await CriarMiniappParaTeste();

            var repositorio = new MiniAppVersaoRepository(DbEscrita, DbLeitura, _mockLoggerRepo.Object);
            var servico = new MiniappVersaoService(repositorio, _mockLoggerService.Object);
            var controller = new VersaoMiniappController(_mockLoggerController.Object, servico);

            var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
            {
                CoMiniapp = coMiniapp,
                NuVersao = nuVersao,
                PcExpansao = pcExpansao,
                IcAtivo = icAtivo
            };

            // Act
            var resultado = await controller.PostVersaoMiniapp(entradaCriarVersao);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(resultado);
            var versaoCriada = Assert.IsType<RetornoCriarVersaoMiniappDto>(createdResult.Value);

            Assert.Equal(nuVersao ?? 1.0m, versaoCriada.NuVersao);
            Assert.Equal(pcExpansao ?? 0.0m, versaoCriada.PcExpansao);
            Assert.Equal(icAtivo, versaoCriada.IcAtivo);
            Assert.Contains(edVersaoEsperada, versaoCriada.EdVersaoMiniapp);
        }
    }
}
