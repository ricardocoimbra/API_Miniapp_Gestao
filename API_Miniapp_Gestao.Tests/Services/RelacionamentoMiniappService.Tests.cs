using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API_Miniapp_Gestao.Tests.Services;

public class RelacionamentoMiniappServiceTests
{
    private readonly Mock<IRelacionamentoMiniappRepository> _repositoryMock;
    private readonly Mock<ILogger<RelacionamentoMiniappService>> _loggerMock;
    private readonly RelacionamentoMiniappService _service;

    private readonly Guid _miniappPaiId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _miniappFilhoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _miniappInexistenteId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    public RelacionamentoMiniappServiceTests()
    {
        _repositoryMock = new Mock<IRelacionamentoMiniappRepository>();
        _loggerMock = new Mock<ILogger<RelacionamentoMiniappService>>();
        _service = new RelacionamentoMiniappService(_repositoryMock.Object, _loggerMock.Object);
    }

    #region CriarRelacionamentoAsync Tests

    [Fact]
    public async Task CriarRelacionamentoAsync_ComDadosValidos_DeveCriarRelacionamento()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappPaiId,
            CoMiniappFilho = _miniappFilhoId
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(false);

        // Act
        await _service.CriarRelacionamentoAsync(relacionamento);

        // Assert
        _repositoryMock.Verify(r => r.CriarRelacionamentoAsync(relacionamento), Times.Once);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComEntradaNula_DeveLancarArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CriarRelacionamentoAsync(null));
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComMiniappPaiVazio_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = Guid.Empty,
            CoMiniappFilho = _miniappFilhoId
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("MINIAPP_PAI_INVALIDO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComMiniappFilhoVazio_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappPaiId,
            CoMiniappFilho = Guid.Empty
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("MINIAPP_FILHO_INVALIDO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComAutorelacionamento_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappPaiId,
            CoMiniappFilho = _miniappPaiId
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("MINIAPP_AUTORELACIONAMENTO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComMiniappPaiInexistente_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappInexistenteId,
            CoMiniappFilho = _miniappFilhoId
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappInexistenteId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("MINIAPP_PAI_NAO_ENCONTRADO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComMiniappFilhoInexistente_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappPaiId,
            CoMiniappFilho = _miniappInexistenteId
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappInexistenteId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("MINIAPP_FILHO_NAO_ENCONTRADO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task CriarRelacionamentoAsync_ComRelacionamentoJaExistente_DeveLancarBusinessException()
    {
        // Arrange
        var relacionamento = new IncluirRelacionamentoMiniappDto
        {
            CoMiniappPai = _miniappPaiId,
            CoMiniappFilho = _miniappFilhoId
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarRelacionamentoAsync(relacionamento));
        Assert.Equal("RELACIONAMENTO_JA_EXISTE", exception.Erro?.Codigo);
    }

    #endregion

    #region ExcluirRelacionamentoAsync Tests

    [Fact]
    public async Task ExcluirRelacionamentoAsync_ComDadosValidos_DeveExcluirRelacionamento()
    {
        // Arrange
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);

        // Act
        await _service.ExcluirRelacionamentoAsync(_miniappPaiId, _miniappFilhoId);

        // Assert
        _repositoryMock.Verify(r => r.ExcluirRelacionamentoAsync(_miniappPaiId, _miniappFilhoId), Times.Once);
    }

    [Fact]
    public async Task ExcluirRelacionamentoAsync_ComMiniappPaiVazio_DeveLancarBusinessException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ExcluirRelacionamentoAsync(Guid.Empty, _miniappFilhoId));
        Assert.Equal("MINIAPP_PAI_INVALIDO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task ExcluirRelacionamentoAsync_ComMiniappFilhoVazio_DeveLancarBusinessException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ExcluirRelacionamentoAsync(_miniappPaiId, Guid.Empty));
        Assert.Equal("MINIAPP_FILHO_INVALIDO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task ExcluirRelacionamentoAsync_ComRelacionamentoInexistente_DeveLancarBusinessException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ExcluirRelacionamentoAsync(_miniappPaiId, _miniappFilhoId));
        Assert.Equal("RELACIONAMENTO_NAO_ENCONTRADO", exception.Erro?.Codigo);
    }

    #endregion

    #region EditarRelacionamentoAsync Tests

    [Fact]
    public async Task EditarRelacionamentoAsync_ComDadosValidos_DeveEditarRelacionamento()
    {
        // Arrange
        var novoMiniappPaiId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var novoMiniappFilhoId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPaiId,
            CoMiniappFilhoOriginal = _miniappFilhoId,
            CoMiniappPaiNovo = novoMiniappPaiId,
            CoMiniappFilhoNovo = novoMiniappFilhoId
        };

        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(novoMiniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(novoMiniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(novoMiniappPaiId, novoMiniappFilhoId)).ReturnsAsync(false);

        // Act
        await _service.EditarRelacionamentoAsync(entrada);

        // Assert
        _repositoryMock.Verify(r => r.CriarRelacionamentoAsync(It.IsAny<IncluirRelacionamentoMiniappDto>()), Times.Once);
        _repositoryMock.Verify(r => r.ExcluirRelacionamentoAsync(_miniappPaiId, _miniappFilhoId), Times.Once);
    }

    [Fact]
    public async Task EditarRelacionamentoAsync_ComEntradaNula_DeveLancarArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.EditarRelacionamentoAsync(null));
    }

    [Fact]
    public async Task EditarRelacionamentoAsync_SemMudancas_DeveLancarBusinessException()
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPaiId,
            CoMiniappFilhoOriginal = _miniappFilhoId,
            CoMiniappPaiNovo = _miniappPaiId,
            CoMiniappFilhoNovo = _miniappFilhoId
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.EditarRelacionamentoAsync(entrada));
        Assert.Equal("SEM_MUDANCAS", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task EditarRelacionamentoAsync_ComAutorelacionamentoNovo_DeveLancarBusinessException()
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPaiId,
            CoMiniappFilhoOriginal = _miniappFilhoId,
            CoMiniappPaiNovo = _miniappPaiId,
            CoMiniappFilhoNovo = _miniappPaiId
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.EditarRelacionamentoAsync(entrada));
        Assert.Equal("MINIAPP_AUTORELACIONAMENTO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task EditarRelacionamentoAsync_ComRelacionamentoOriginalInexistente_DeveLancarBusinessException()
    {
        // Arrange
        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPaiId,
            CoMiniappFilhoOriginal = _miniappFilhoId,
            CoMiniappPaiNovo = Guid.NewGuid(),
            CoMiniappFilhoNovo = Guid.NewGuid()
        };

        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.EditarRelacionamentoAsync(entrada));
        Assert.Equal("RELACIONAMENTO_NAO_ENCONTRADO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task EditarRelacionamentoAsync_ComNovoRelacionamentoJaExistente_DeveLancarBusinessException()
    {
        // Arrange
        var novoMiniappPaiId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var novoMiniappFilhoId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var entrada = new EditarRelacionamentoMiniappDto
        {
            CoMiniappPaiOriginal = _miniappPaiId,
            CoMiniappFilhoOriginal = _miniappFilhoId,
            CoMiniappPaiNovo = novoMiniappPaiId,
            CoMiniappFilhoNovo = novoMiniappFilhoId
        };

        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(novoMiniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(novoMiniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(novoMiniappPaiId, novoMiniappFilhoId)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.EditarRelacionamentoAsync(entrada));
        Assert.Equal("RELACIONAMENTO_JA_EXISTE", exception.Erro?.Codigo);
    }

    #endregion

    #region ListarRelacionamentosAsync Tests

    [Fact]
    public async Task ListarRelacionamentosAsync_ComDadosValidos_DeveRetornarLista()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto
        {
            CoMiniapp = _miniappPaiId,
            Relacao = "AMBOS"
        };

        var paisEsperados = new List<RetornoRelacionamentosDto>
        {
            new() { CoMiniapp = Guid.NewGuid(), NoMiniapp = "Pai 1" }
        };

        var filhosEsperados = new List<RetornoRelacionamentosDto>
        {
            new() { CoMiniapp = Guid.NewGuid(), NoMiniapp = "Filho 1" }
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetMiniappsPaisAsync(_miniappPaiId)).ReturnsAsync(paisEsperados);
        _repositoryMock.Setup(r => r.GetMiniappsFilhosAsync(_miniappPaiId)).ReturnsAsync(filhosEsperados);

        // Act
        var resultado = await _service.ListarRelacionamentosAsync(entrada);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(paisEsperados.Count, resultado.pais.Count);
        Assert.Equal(filhosEsperados.Count, resultado.filhos.Count);
    }

    [Fact]
    public async Task ListarRelacionamentosAsync_ComEntradaNula_DeveLancarArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ListarRelacionamentosAsync(null));
    }

    [Fact]
    public async Task ListarRelacionamentosAsync_ComMiniappVazio_DeveLancarBusinessException()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto
        {
            CoMiniapp = Guid.Empty,
            Relacao = "AMBOS"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ListarRelacionamentosAsync(entrada));
        Assert.Equal("MINIAPP_INVALIDO", exception.Erro?.Codigo);
    }

    [Fact]
    public async Task ListarRelacionamentosAsync_ComMiniappInexistente_DeveLancarBusinessException()
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto
        {
            CoMiniapp = _miniappInexistenteId,
            Relacao = "AMBOS"
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappInexistenteId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.ListarRelacionamentosAsync(entrada));
        Assert.Equal("MINIAPP_NAO_ENCONTRADO", exception.Erro?.Codigo);
    }

    [Theory]
    [InlineData("PAIS")]
    [InlineData("FILHOS")]
    [InlineData("AMBOS")]
    [InlineData("")]
    [InlineData(null)]
    public async Task ListarRelacionamentosAsync_ComDiferentesTiposRelacao_DeveProcessarCorretamente(string tipoRelacao)
    {
        // Arrange
        var entrada = new EntradaRelacionamentosDto
        {
            CoMiniapp = _miniappPaiId,
            Relacao = tipoRelacao
        };

        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetMiniappsPaisAsync(_miniappPaiId)).ReturnsAsync(new List<RetornoRelacionamentosDto>());
        _repositoryMock.Setup(r => r.GetMiniappsFilhosAsync(_miniappPaiId)).ReturnsAsync(new List<RetornoRelacionamentosDto>());

        // Act
        var resultado = await _service.ListarRelacionamentosAsync(entrada);

        // Assert
        Assert.NotNull(resultado);

        // Verificar se os métodos corretos foram chamados baseados no tipo de relação
        switch (tipoRelacao?.ToUpper())
        {
            case "PAIS":
                _repositoryMock.Verify(r => r.GetMiniappsPaisAsync(_miniappPaiId), Times.Once);
                _repositoryMock.Verify(r => r.GetMiniappsFilhosAsync(_miniappPaiId), Times.Never);
                break;
            case "FILHOS":
                _repositoryMock.Verify(r => r.GetMiniappsPaisAsync(_miniappPaiId), Times.Never);
                _repositoryMock.Verify(r => r.GetMiniappsFilhosAsync(_miniappPaiId), Times.Once);
                break;
            default: // AMBOS, null, ou string vazia
                _repositoryMock.Verify(r => r.GetMiniappsPaisAsync(_miniappPaiId), Times.Once);
                _repositoryMock.Verify(r => r.GetMiniappsFilhosAsync(_miniappPaiId), Times.Once);
                break;
        }
    }

    #endregion

    #region ValidarRelacionamentoAsync Tests

    [Fact]
    public async Task ValidarRelacionamentoAsync_ComAutorelacionamento_DeveRetornarFalse()
    {
        // Act
        var resultado = await _service.ValidarRelacionamentoAsync(_miniappPaiId, _miniappPaiId);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ValidarRelacionamentoAsync_ComMiniappPaiInexistente_DeveRetornarFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappInexistenteId)).ReturnsAsync(false);

        // Act
        var resultado = await _service.ValidarRelacionamentoAsync(_miniappInexistenteId, _miniappFilhoId);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ValidarRelacionamentoAsync_ComMiniappFilhoInexistente_DeveRetornarFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappInexistenteId)).ReturnsAsync(false);

        // Act
        var resultado = await _service.ValidarRelacionamentoAsync(_miniappPaiId, _miniappInexistenteId);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ValidarRelacionamentoAsync_ComRelacionamentoJaExistente_DeveRetornarFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);

        // Act
        var resultado = await _service.ValidarRelacionamentoAsync(_miniappPaiId, _miniappFilhoId);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ValidarRelacionamentoAsync_ComDadosValidos_DeveRetornarTrue()
    {
        // Arrange
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappPaiId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.MiniappExisteAsync(_miniappFilhoId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(false);

        // Act
        var resultado = await _service.ValidarRelacionamentoAsync(_miniappPaiId, _miniappFilhoId);

        // Assert
        Assert.True(resultado);
    }

    #endregion

    #region RelacionamentoExisteAsync Tests

    [Fact]
    public async Task RelacionamentoExisteAsync_ComRelacionamentoExistente_DeveRetornarTrue()
    {
        // Arrange
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(true);

        // Act
        var resultado = await _service.RelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task RelacionamentoExisteAsync_ComRelacionamentoInexistente_DeveRetornarFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.VerificarRelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId)).ReturnsAsync(false);

        // Act
        var resultado = await _service.RelacionamentoExisteAsync(_miniappPaiId, _miniappFilhoId);

        // Assert
        Assert.False(resultado);
    }

    #endregion
}
