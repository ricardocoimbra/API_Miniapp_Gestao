

using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services;
using API_Miniapp_Gestao.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API_Miniapp_Gestao.Tests.Services;

public class MiniappServiceTests
{
   private readonly Mock<IMiniAppRepository> _miniAppRepositoryMock;
   private readonly Mock<IMiniAppVersaoRepository> _miniAppVersaoRepositoryMock;
   private readonly IMiniAppService _miniAppService;
   private readonly RetornoMiniappDto _retornoMiniapp01;
   private readonly RetornoMiniappDto _retornoMiniapp02;

   public MiniappServiceTests()
   {
      _miniAppRepositoryMock = new Mock<IMiniAppRepository>();
      _miniAppVersaoRepositoryMock = new Mock<IMiniAppVersaoRepository>();

      var loggerMock = new Mock<ILogger<MiniAppService>>();

      _miniAppService = new MiniAppService(_miniAppRepositoryMock.Object, _miniAppVersaoRepositoryMock.Object, loggerMock.Object);

      _retornoMiniapp01 = new RetornoMiniappDto()
      {
         Miniapp = new MiniappDto()
         {
            CoMiniapp = Guid.Parse("71aeb9b8-2048-482d-a750-27693c79eaa8"),
            NoMiniapp = "module_home",
            NoApelidoMiniapp = "module_home",
            DeMiniapp = "Home do App",
            IcMiniappInicial = true,
            IcAtivo = true
         },
         VersoesMiniapp = new List<VersaoMiniappDto>()
         {
            new VersaoMiniappDto()
            {
               CoVersao = Guid.Parse("d2c99728-404f-4744-90d5-0c6411acba5f"),
               NuVersao = 0.990M,
               PcExpansao = 1.000M,
               IcAtivo = true
            }
         }
      };

      _retornoMiniapp02 = new RetornoMiniappDto()
      {
         Miniapp = new MiniappDto()
         {
            CoMiniapp = Guid.Parse("36474ced-fcf9-4c93-b470-a5f141293d2c"),
            NoMiniapp = "module_cartao_debito",
            NoApelidoMiniapp = "module_cartao_debito",
            DeMiniapp = "Cartao de Debito",
            IcMiniappInicial = false,
            IcAtivo = true
         },
         VersoesMiniapp = new List<VersaoMiniappDto>()
         {
            new VersaoMiniappDto()
            {
               CoVersao = Guid.Parse("ca64afd3-7936-44ab-9e2b-026e4a429491"),
               NuVersao = 2.100M,
               PcExpansao = 0.020M,
               IcAtivo = true
            }
         }
      };
   }

   #region TESTES CONSULTAR MINIAPPS
   // SUCESSO
   [Fact]
   public async Task ConsultarMiniapps_ShouldReturnAllMiniapps_WhenNoCoMiniappProvided()
   {
      // Arrange
      var entrada = new EntradaMiniappDto(); // Sem CoMiniapp

      var listaMiniapps = new List<RetornoMiniappDto> { _retornoMiniapp01, _retornoMiniapp02 };

      _miniAppRepositoryMock.Setup(repo => repo.GetMiniAppsAsync())
         .ReturnsAsync(listaMiniapps.Select(r => r.Miniapp).ToList());

      _miniAppRepositoryMock.Setup(repo => repo.GetVersoesMiniappPorCoMiniappAsync(It.IsAny<Guid>()))
         .ReturnsAsync((Guid coMiniapp) =>
         {
            return listaMiniapps.First(r => r.Miniapp.CoMiniapp == coMiniapp).VersoesMiniapp;
         });

      // Act
      var result = await _miniAppService.ConsultaMiniapps(entrada);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(2, result.Count);
      Assert.Contains(result, r => r.Miniapp.CoMiniapp == _retornoMiniapp01.Miniapp.CoMiniapp);
      Assert.Contains(result, r => r.Miniapp.CoMiniapp == _retornoMiniapp02.Miniapp.CoMiniapp);
   }

   [Fact]
   public async Task ConsultarMiniapps_ShouldReturnSpecificMiniapp_WhenCoMiniappIsProvided()
   {
      // Arrange
      const string CO_MINIAPP_RETORNO_01 = "71aeb9b8-2048-482d-a750-27693c79eaa8";
      var entrada = new EntradaMiniappDto { CoMiniapp = CO_MINIAPP_RETORNO_01 };

      var listaMiniapps = new List<RetornoMiniappDto> { _retornoMiniapp01 };
      var guidCominiapp = Guid.Parse(CO_MINIAPP_RETORNO_01);

      _miniAppRepositoryMock.Setup(repo => repo.GetMiniAppByCoMiniappAsync(guidCominiapp))
         .ReturnsAsync(listaMiniapps.First(r => r.Miniapp.CoMiniapp == guidCominiapp).Miniapp);

      _miniAppRepositoryMock.Setup(repo => repo.GetVersoesMiniappPorCoMiniappAsync(It.IsAny<Guid>()))
         .ReturnsAsync((Guid coMiniapp) =>
         {
            return listaMiniapps.First(r => r.Miniapp.CoMiniapp == coMiniapp).VersoesMiniapp;
         });

      // Act
      var result = await _miniAppService.ConsultaMiniapps(entrada);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Equal(_retornoMiniapp01.Miniapp.CoMiniapp, result[0].Miniapp.CoMiniapp);
   }

   // FALHA
   [Fact]
   public async Task ConsultarMiniapps_ShouldReturnNull_WhenCoMiniappDoesNotExist()
   {
      // Arrange
      const string CO_MINIAPP_INEXISTENTE = "71aeb9b8-2048-482d-a750-27693c790008";
      var entrada = new EntradaMiniappDto { CoMiniapp = CO_MINIAPP_INEXISTENTE };

      var guidCominiapp = Guid.Parse(CO_MINIAPP_INEXISTENTE);

      _miniAppRepositoryMock.Setup(repo => repo.GetMiniAppByCoMiniappAsync(guidCominiapp))
         .ReturnsAsync((MiniappDto)null!);

      // Act
      var result = await _miniAppService.ConsultaMiniapps(entrada);

      // Assert
      Assert.Null(result);
   }

   [Fact]
   public async Task ConsultarMiniapps_ShouldThrowBusinessException_WhenRepositoryThrowsException()
   {
      // Arrange
      var entrada = new EntradaMiniappDto(); // Sem CoMiniapp

      _miniAppRepositoryMock.Setup(repo => repo.GetMiniAppsAsync())
         .ThrowsAsync(new Exception("Database error"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<BusinessException>(() => _miniAppService.ConsultaMiniapps(entrada));
      Assert.Equal("Listar Miniapps", exception.Erro.Codigo);
      Assert.Equal("Erro ao listar Miniapps", exception.Erro.Mensagem);
      Assert.Equal(500, exception.Erro.StatusCode);
   }

   #endregion TESTES CONSULTAR MINIAPPS

   #region TESTES CRIAR MINIAPP
   // SUCESSO
   [Fact]
   public async Task CriarMiniapp_ShouldReturnCreatedMiniapp_WhenValidInputIsProvided()
   {
      // Arrange
      var entrada = new CriarMiniappDto
      {
         NoMiniapp = "module_extrato",
         NoApelidoMiniapp = "module_extrato",
         DeMiniapp = "Extrato do App",
         IcMiniappInicial = false,
         IcAtivo = true
      };

      // Act
      var result = await _miniAppService.CriarMiniapp(entrada);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(entrada.NoMiniapp, result.NoMiniapp);
      Assert.Equal(entrada.NoApelidoMiniapp, result.NoApelidoMiniapp);
      Assert.Equal(entrada.DeMiniapp, result.DeMiniapp);
      Assert.Equal(entrada.IcMiniappInicial, result.IcMiniappInicial);
      Assert.Equal(entrada.IcAtivo, result.IcAtivo);
      Assert.NotEqual(Guid.Empty, result.CoMiniapp); // Verifica se um novo GUID foi gerado

      _miniAppRepositoryMock.Verify(repo => repo.CreateMiniAppAsync(It.Is<MiniappDto>(m =>
         m.NoMiniapp == entrada.NoMiniapp &&
         m.NoApelidoMiniapp == entrada.NoApelidoMiniapp &&
         m.DeMiniapp == entrada.DeMiniapp &&
         m.IcMiniappInicial == entrada.IcMiniappInicial &&
         m.IcAtivo == entrada.IcAtivo
      )), Times.Once);
   }

   // FALHA
   [Fact]
   public async Task CriarMiniapp_ShouldThrowBusinessException_WhenRepoThrowsAnException()
   {
      // Arrange
      var entrada = new CriarMiniappDto
      {
         NoMiniapp = "module_home",
         NoApelidoMiniapp = "module_home",
         DeMiniapp = "Home do App",
         IcMiniappInicial = true,
         IcAtivo = true
      };

      var exception = new BusinessException("Criar Miniapp", "Erro ao criar Miniapp", 500);

      _miniAppRepositoryMock.Setup(repo => repo.CreateMiniAppAsync(It.IsAny<MiniappDto>()))
         .Throws(exception);

      // Act & Assert
      var exceptionThrown = await Assert.ThrowsAsync<BusinessException>(() => Task.Run(() => _miniAppService.CriarMiniapp(entrada)));
      Assert.Equal("Criar Miniapps", exceptionThrown.Erro.Codigo);
      Assert.Equal("Erro ao criar Miniapp", exceptionThrown.Erro.Mensagem);
      Assert.Equal(500, exceptionThrown.Erro.StatusCode);

      _miniAppRepositoryMock.Verify(repo => repo.CreateMiniAppAsync(It.IsAny<MiniappDto>()), Times.Once);
   }

   [Fact]
   public async Task MiniappExists_ShouldReturnTrue_WhenMiniappNameExists()
   {
      // Arrange
      const string NOME_MINIAPP_EXISTENTE = "module_home";

      _miniAppRepositoryMock.Setup(repo => repo.GetMiniAppByNameAsync(NOME_MINIAPP_EXISTENTE))
         .ReturnsAsync(new MiniappDto
         {
            CoMiniapp = Guid.NewGuid(),
            NoMiniapp = NOME_MINIAPP_EXISTENTE,
            NoApelidoMiniapp = "module_home",
            DeMiniapp = "Home do App",
            IcMiniappInicial = true,
            IcAtivo = true
         });

      // Act
      var result = await _miniAppService.MiniappExists(NOME_MINIAPP_EXISTENTE);

      // Assert
      Assert.True(result);

      _miniAppRepositoryMock.Verify(repo => repo.GetMiniAppByNameAsync(NOME_MINIAPP_EXISTENTE), Times.Once);
   }

   #endregion TESTES CRIAR MINIAPP
}
