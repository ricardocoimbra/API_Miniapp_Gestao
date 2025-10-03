using API_Miniapp_Gestao.DTO;
using Xunit;

namespace API_Miniapp_Gestao.Tests.DTO;

public class EditarRelacionamentoMiniappDtoTests
{
    private readonly Guid _miniappPai1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _miniappFilho1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _miniappPai2 = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly Guid _miniappFilho2 = Guid.Parse("44444444-4444-4444-4444-444444444444");

    #region TemMudancas Tests

    [Fact]
    public void TemMudancas_QuandoNaoHaMudancas_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai1,
            CoMiniappFilhoNovo = _miniappFilho1
        };

        // Act & Assert
        Assert.False(dto.TemMudancas);
    }

    [Fact]
    public void TemMudancas_QuandoMudaApenasPai_DeveRetornarTrue()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho1
        };

        // Act & Assert
        Assert.True(dto.TemMudancas);
    }

    [Fact]
    public void TemMudancas_QuandoMudaApenasFilho_DeveRetornarTrue()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai1,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.True(dto.TemMudancas);
    }

    [Fact]
    public void TemMudancas_QuandoMudaAmbos_DeveRetornarTrue()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.True(dto.TemMudancas);
    }

    #endregion

    #region GUIDsValidos Tests

    [Fact]
    public void GUIDsValidos_QuandoTodosGUIDsSaoValidos_DeveRetornarTrue()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.True(dto.GUIDsValidos);
    }

    [Fact]
    public void GUIDsValidos_QuandoMiniappPaiOriginalEhEmpty_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = Guid.Empty,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos);
    }

    [Fact]
    public void GUIDsValidos_QuandoMiniappFilhoOriginalEhEmpty_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = Guid.Empty,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos);
    }

    [Fact]
    public void GUIDsValidos_QuandoMiniappPaiNovoEhEmpty_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = Guid.Empty,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos);
    }

    [Fact]
    public void GUIDsValidos_QuandoMiniappFilhoNovoEhEmpty_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = Guid.Empty
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos);
    }

    [Fact]
    public void GUIDsValidos_QuandoTodosGUIDsSaoEmpty_DeveRetornarFalse()
    {
        // Arrange
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = Guid.Empty,
            CoMiniappFilhoOriginal = Guid.Empty,
            CoMiniappPaiNovo = Guid.Empty,
            CoMiniappFilhoNovo = Guid.Empty
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos);
    }

    #endregion

    #region Cenários Combinados

    [Fact]
    public void PropriedadesCalculadas_CenarioComum_EditorValida()
    {
        // Arrange - Cenário típico: mudando pai e filho
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai2,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.True(dto.GUIDsValidos, "GUIDs devem ser válidos");
        Assert.True(dto.TemMudancas, "Deve ter mudanças");
    }

    [Fact]
    public void PropriedadesCalculadas_CenarioSemMudancas_MasGUIDsValidos()
    {
        // Arrange - Cenário onde usuário envia dados iguais
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = _miniappPai1,
            CoMiniappFilhoNovo = _miniappFilho1
        };

        // Act & Assert
        Assert.True(dto.GUIDsValidos, "GUIDs devem ser válidos");
        Assert.False(dto.TemMudancas, "Não deve ter mudanças");
    }

    [Fact]
    public void PropriedadesCalculadas_CenarioComMudancas_MasGUIDsInvalidos()
    {
        // Arrange - Cenário com erro de validação
        var dto = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPai1,
            CoMiniappFilhoOriginal = _miniappFilho1,
            CoMiniappPaiNovo = Guid.Empty,
            CoMiniappFilhoNovo = _miniappFilho2
        };

        // Act & Assert
        Assert.False(dto.GUIDsValidos, "GUIDs não devem ser válidos");
        Assert.True(dto.TemMudancas, "Deve ter mudanças");
    }

    #endregion
}
