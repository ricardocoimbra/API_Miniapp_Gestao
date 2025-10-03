using API_Miniapp_Gestao.Controllers;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Services.Interfaces;
using API_Miniapp_Gestao.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;


namespace API_Miniapp_Gestao.Tests.Controllers;

public class MiniappControllerTests
{
   private readonly Mock<IMiniAppService> _miniAppServiceMock;
   private readonly RetornoMiniappDto _retornoMiniapp01;
   private readonly RetornoMiniappDto _retornoMiniapp02;

   public MiniappControllerTests()
   {
      _miniAppServiceMock = new Mock<IMiniAppService>();

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

   #region TESTES LISTAR MINIAPPS

   // SUCESSO
   [Fact]
   public async Task GetListarMiniapps_ShouldReturnsOkResultWithListOfMiniapps_WhenCoMiniappIsNullOrEmpty()
   {
      // Arrange
      var entrada = new EntradaMiniappDto() { CoMiniapp = null };
      var expectedMiniapps = new List<RetornoMiniappDto> { _retornoMiniapp01, _retornoMiniapp02 };

      _miniAppServiceMock.Setup(service => service.ConsultaMiniapps(entrada))
         .ReturnsAsync(expectedMiniapps);

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act
      var result = await _controller.GetListarMiniapps(entrada);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnValue = Assert.IsType<List<RetornoMiniappDto>>(okResult.Value);
      Assert.Equal(expectedMiniapps, returnValue);
   }

   [Fact]
   public async Task GetListarMiniapps_ShouldReturnsOkResultWithListWithSelectedMiniapp_WhenCoMiniappIsGiven()
   {
      // Arrange
      const string CO_MINIAPP_RETORNO_01 = "71aeb9b8-2048-482d-a750-27693c79eaa8";
      var entrada = new EntradaMiniappDto() { CoMiniapp = CO_MINIAPP_RETORNO_01 };
      var expectedMiniapps = new List<RetornoMiniappDto> { _retornoMiniapp01 };

      _miniAppServiceMock.Setup(service => service.ConsultaMiniapps(entrada))
         .ReturnsAsync(expectedMiniapps);

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act
      var result = await _controller.GetListarMiniapps(entrada);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnValue = Assert.IsType<List<RetornoMiniappDto>>(okResult.Value);
      Assert.Single(returnValue);
      Assert.Equal(expectedMiniapps, returnValue);
   }

   // FALHA
   [Fact]
   public async Task GetListarMiniapps_ShouldReturnsNotFoundResult_WhenCoMiniappNotExists()
   {
      // Arrange
      var entrada = new EntradaMiniappDto() { CoMiniapp = "invalid-guid" };

      _miniAppServiceMock.Setup(service => service.ConsultaMiniapps(entrada))
         .ReturnsAsync(new List<RetornoMiniappDto>());

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act
      var result = await _controller.GetListarMiniapps(entrada);

      // Assert
      Assert.IsType<NotFoundObjectResult>(result.Result);
   }

   #endregion TESTES LISTAR MINIAPPS

   #region TESTES CRIAR MINIAPP
   // SUCESSO
   [Fact]
   public async Task PostMiniapp_ShouldReturnsCreatedResultWithCreatedMiniapp_WhenDataIsValid()
   {
      // Arrange
      var entrada = new CriarMiniappDto()
      {
         NoMiniapp = "module_investimentos",
         NoApelidoMiniapp = "module_investimentos",
         DeMiniapp = "Investimentos",
         IcMiniappInicial = false,
         IcAtivo = true
      };

      var createdMiniapp = new MiniappDto()
      {
         CoMiniapp = Guid.NewGuid(),
         NoMiniapp = entrada.NoMiniapp!,
         NoApelidoMiniapp = entrada.NoApelidoMiniapp!,
         DeMiniapp = entrada.DeMiniapp,
         IcMiniappInicial = entrada.IcMiniappInicial,
         IcAtivo = entrada.IcAtivo
      };

      _miniAppServiceMock.Setup(service => service.MiniappExists(entrada.NoMiniapp!))
         .ReturnsAsync(false);

      _miniAppServiceMock.Setup(service => service.CriarMiniapp(entrada))
         .ReturnsAsync(createdMiniapp);

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act
      var result = await _controller.PostMiniapp(entrada);

      // Assert
      var createdResult = Assert.IsType<ObjectResult>(result);
      Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
      var returnValue = Assert.IsType<MiniappDto>(createdResult.Value);
      Assert.Equal(createdMiniapp, returnValue);
   }

   // FALHA
   [Fact]
   public async Task PostMiniapp_ShouldReturnsConflictResult_WhenMiniappNameAlreadyExists()
   {
      // Arrange
      var entrada = new CriarMiniappDto()
      {
         NoMiniapp = "module_home",
         NoApelidoMiniapp = "module_home",
         DeMiniapp = "Home do App",
         IcMiniappInicial = true,
         IcAtivo = true
      };

      _miniAppServiceMock.Setup(service => service.MiniappExists(entrada.NoMiniapp!))
         .ReturnsAsync(true);

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act
      var result = await _controller.PostMiniapp(entrada);

      // Assert
      var conflictResult = Assert.IsType<ConflictObjectResult>(result);
      Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
   }

   [Fact]
   public async Task PostMiniapp_ShouldReturnsBadRequestResult_WhenDataIsInvalid()
   {
      // Arrange
      var entrada = new CriarMiniappDto()
      {
         NoMiniapp = "", // Nome inválido (vazio)
         NoApelidoMiniapp = "module_investimentos",
         DeMiniapp = "Investimentos",
         IcMiniappInicial = false,
         IcAtivo = true
      };

      var _controller = new MiniAppController(_miniAppServiceMock.Object);
      _controller.ModelState.AddModelError("NoMiniapp", "O nome do miniapp deve ser informado.");

      // Act
      var result = await _controller.PostMiniapp(entrada);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
      Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
   }

   [Fact]
   public async Task PostMiniapp_ShouldThrowsBusinessException_WhenServiceThrowsBusinessException()
   {
      // Arrange
      var entrada = new CriarMiniappDto()
      {
         NoMiniapp = "module_investimentos",
         NoApelidoMiniapp = "module_investimentos",
         DeMiniapp = "Investimentos",
         IcMiniappInicial = false,
         IcAtivo = true
      };

      _miniAppServiceMock.Setup(service => service.MiniappExists(entrada.NoMiniapp!))
         .ReturnsAsync(false);

      _miniAppServiceMock.Setup(service => service.CriarMiniapp(entrada))
         .Throws(new BusinessException("Erro ao criar Miniapp", "Erro ao criar Miniapp", 500));

      var _controller = new MiniAppController(_miniAppServiceMock.Object);

      // Act & Assert
      var exception = await Assert.ThrowsAsync<BusinessException>(() => _controller.PostMiniapp(entrada));
      Assert.Equal("Criar Miniapp", exception.Erro.Codigo);
      Assert.Equal("Erro ao criar Miniapp", exception.Erro.Mensagem);
      Assert.Equal(500, exception.Erro.StatusCode);
   }


   #endregion TESTES CRIAR MINIAPP

}
