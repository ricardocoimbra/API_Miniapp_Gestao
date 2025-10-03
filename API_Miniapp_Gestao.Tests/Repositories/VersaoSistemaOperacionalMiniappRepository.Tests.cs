using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.Repositories;

namespace API_Miniapp_Gestao.Tests.Repositories
{
    public class VersaoSistemaOperacionalMiniappRepositoryTests : IDisposable
    {
        private readonly DbEscrita _context;
        private readonly VersaoSistemaOperacionalMiniappRepository _repository;

        public VersaoSistemaOperacionalMiniappRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DbEscrita>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DbEscrita(options);
            _repository = new VersaoSistemaOperacionalMiniappRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region CriarVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task CriarVersaoSistemaOperacionalAsync_ComDadosValidos_DeveCriarVersao()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            // Act
            var resultado = await _repository.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.CoVersaoSistemaOperacional);
            Assert.Equal(entrada.CoPlataforma, resultado.CoPlataforma);
            Assert.Equal(entrada.NuVersaoSistemaOperacional, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSdk, resultado.NuVersaoSdk);

            // Verificar se foi persistido no banco
            var versaoPersistida = await _context.Nbmtb001VersaoSistemaOperacionals
                .FirstOrDefaultAsync(v => v.CoVersaoSistemaOperacional == resultado.CoVersaoSistemaOperacional);

            Assert.NotNull(versaoPersistida);
            Assert.Equal(entrada.CoPlataforma, versaoPersistida.CoPlataforma);
            Assert.Equal(entrada.NuVersaoSistemaOperacional, versaoPersistida.NuVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSdk, versaoPersistida.NuVersaoSdk);
        }

        [Theory]
        [InlineData("A", 12.0, 31.0)]
        [InlineData("I", 15.0, 15.2)]
        public async Task CriarVersaoSistemaOperacionalAsync_ComDiferentesDados_DeveCriarCorretamente(
            string coPlataforma, decimal nuVersaoSO, decimal nuVersaoSdk)
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = coPlataforma,
                NuVersaoSistemaOperacional = nuVersaoSO,
                NuVersaoSdk = nuVersaoSdk
            };

            // Act
            var resultado = await _repository.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.Equal(coPlataforma, resultado.CoPlataforma);
            Assert.Equal(nuVersaoSO, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(nuVersaoSdk, resultado.NuVersaoSdk);
        }

        #endregion

        #region ListarVersoesSistemaOperacionalAsync Tests

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_ComVersoes_DeveRetornarLista()
        {
            // Arrange
            var versao1 = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };
            var versao2 = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 11.0m,
                NuVersaoSdk = 30.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.AddRange(versao1, versao2);
            await _context.SaveChangesAsync();

            var entrada = new ListaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A"
            };

            // Act
            var resultado = await _repository.ListarVersoesSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, v => v.NuVersaoSistemaOperacional == 12.0m);
            Assert.Contains(resultado, v => v.NuVersaoSistemaOperacional == 11.0m);
        }

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_SemFiltros_DeveRetornarTodasVersoes()
        {
            // Arrange
            var versao1 = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };
            var versao2 = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.AddRange(versao1, versao2);
            await _context.SaveChangesAsync();

            var entrada = new ListaVersaoSistemaOperacionalMiniappDto();

            // Act
            var resultado = await _repository.ListarVersoesSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ListarVersoesSistemaOperacionalAsync_ComFiltroPlataforma_DeveRetornarApenasPlataformaFiltrada()
        {
            // Arrange
            var versaoAndroid = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };
            var versaoIOS = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "I",
                NuVersaoSistemaOperacional = 15.0m,
                NuVersaoSdk = 15.2m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.AddRange(versaoAndroid, versaoIOS);
            await _context.SaveChangesAsync();

            var entrada = new ListaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A"
            };

            // Act
            var resultado = await _repository.ListarVersoesSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("A", resultado.First().CoPlataforma);
        }

        #endregion

        #region AtualizarVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task AtualizarVersaoSistemaOperacionalAsync_ComVersaoExistente_DeveAtualizarCorretamente()
        {
            // Arrange
            var versaoExistente = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 11.0m,
                NuVersaoSdk = 30.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(versaoExistente);
            await _context.SaveChangesAsync();

            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = versaoExistente.CoVersaoSistemaOperacional,
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            // Act
            var resultado = await _repository.AtualizarVersaoSistemaOperacionalAsync(
                entrada.CoVersaoSistemaOperacional, entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(entrada.CoVersaoSistemaOperacional, resultado.CoVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSistemaOperacional, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSdk, resultado.NuVersaoSdk);

            // Verificar se foi atualizado no banco
            var versaoAtualizada = await _context.Nbmtb001VersaoSistemaOperacionals
                .FirstOrDefaultAsync(v => v.CoVersaoSistemaOperacional == entrada.CoVersaoSistemaOperacional);

            Assert.NotNull(versaoAtualizada);
            Assert.Equal(entrada.NuVersaoSistemaOperacional, versaoAtualizada.NuVersaoSistemaOperacional);
            Assert.Equal(entrada.NuVersaoSdk, versaoAtualizada.NuVersaoSdk);
        }

        [Fact]
        public async Task AtualizarVersaoSistemaOperacionalAsync_ComVersaoInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var coVersaoInexistente = Guid.NewGuid();
            var entrada = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = coVersaoInexistente,
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _repository.AtualizarVersaoSistemaOperacionalAsync(coVersaoInexistente, entrada));
        }

        #endregion

        #region ExcluirVersaoSistemaOperacionalAsync Tests

        [Fact]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComVersaoExistente_DeveExcluirCorretamente()
        {
            // Arrange
            var versaoExistente = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(versaoExistente);
            await _context.SaveChangesAsync();

            // Act
            await _repository.ExcluirVersaoSistemaOperacionalAsync(versaoExistente.CoVersaoSistemaOperacional);

            // Assert
            var versaoExcluida = await _context.Nbmtb001VersaoSistemaOperacionals
                .FirstOrDefaultAsync(v => v.CoVersaoSistemaOperacional == versaoExistente.CoVersaoSistemaOperacional);

            Assert.Null(versaoExcluida);
        }

        [Fact]
        public async Task ExcluirVersaoSistemaOperacionalAsync_ComVersaoInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var coVersaoInexistente = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _repository.ExcluirVersaoSistemaOperacionalAsync(coVersaoInexistente));
        }

        #endregion

        #region VersaoSistemaOperacionalExisteAsync Tests

        [Fact]
        public async Task VersaoSistemaOperacionalExisteAsync_ComVersaoExistente_DeveRetornarTrue()
        {
            // Arrange
            var versaoExistente = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(versaoExistente);
            await _context.SaveChangesAsync();

            // Act
            var existe = await _repository.VersaoSistemaOperacionalExisteAsync(versaoExistente.CoVersaoSistemaOperacional);

            // Assert
            Assert.True(existe);
        }

        [Fact]
        public async Task VersaoSistemaOperacionalExisteAsync_ComVersaoInexistente_DeveRetornarFalse()
        {
            // Arrange
            var coVersaoInexistente = Guid.NewGuid();

            // Act
            var existe = await _repository.VersaoSistemaOperacionalExisteAsync(coVersaoInexistente);

            // Assert
            Assert.False(existe);
        }

        #endregion

        #region VerificarDuplicacaoAsync Tests

        [Fact]
        public async Task VerificarDuplicacaoAsync_ComVersaoExistente_DeveRetornarTrue()
        {
            // Arrange
            var versaoExistente = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(versaoExistente);
            await _context.SaveChangesAsync();

            // Act
            var duplicacao = await _repository.VerificarDuplicacaoAsync("A", 12.0m);

            // Assert
            Assert.True(duplicacao);
        }

        [Fact]
        public async Task VerificarDuplicacaoAsync_ComVersaoInexistente_DeveRetornarFalse()
        {
            // Act
            var duplicacao = await _repository.VerificarDuplicacaoAsync("A", 13.0m);

            // Assert
            Assert.False(duplicacao);
        }

        [Fact]
        public async Task VerificarDuplicacaoAsync_ExcluindoVersaoAtual_DeveRetornarFalse()
        {
            // Arrange
            var versaoExistente = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = Guid.NewGuid(),
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(versaoExistente);
            await _context.SaveChangesAsync();

            // Act - Verificar duplicação excluindo a própria versão
            var duplicacao = await _repository.VerificarDuplicacaoAsync("A", 12.0m, versaoExistente.CoVersaoSistemaOperacional);

            // Assert
            Assert.False(duplicacao);
        }

        #endregion

        #region Testes Adicionais de Validação

        [Fact]
        public async Task CriarVersaoSistemaOperacionalAsync_ComPlataformaVazia_DeveCriarCorretamente()
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            // Act
            var resultado = await _repository.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("", resultado.CoPlataforma);
        }

        [Theory]
        [InlineData(0.0, 1.0)]
        [InlineData(1.0, 0.0)]
        public async Task CriarVersaoSistemaOperacionalAsync_ComValoresLimite_DeveCriarCorretamente(
            decimal nuVersaoSO, decimal nuVersaoSdk)
        {
            // Arrange
            var entrada = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = nuVersaoSO,
                NuVersaoSdk = nuVersaoSdk
            };

            // Act
            var resultado = await _repository.CriarVersaoSistemaOperacionalAsync(entrada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nuVersaoSO, resultado.NuVersaoSistemaOperacional);
            Assert.Equal(nuVersaoSdk, resultado.NuVersaoSdk);
        }

        #endregion

        #region Testes de Integração

        [Fact]
        public async Task FluxoCompleto_CriarListarAtualizarExcluir_DeveExecutarCorretamente()
        {
            // Arrange
            var entradaCriacao = new EntradaVersaoSistemaOperacionalMiniappDto
            {
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 12.0m,
                NuVersaoSdk = 31.0m
            };

            // Act & Assert - Criar
            var versaoCriada = await _repository.CriarVersaoSistemaOperacionalAsync(entradaCriacao);
            Assert.NotNull(versaoCriada);
            Assert.NotEqual(Guid.Empty, versaoCriada.CoVersaoSistemaOperacional);

            // Act & Assert - Listar
            var entradaListagem = new ListaVersaoSistemaOperacionalMiniappDto();
            var versoes = await _repository.ListarVersoesSistemaOperacionalAsync(entradaListagem);
            Assert.Single(versoes);

            // Act & Assert - Atualizar
            var entradaAtualizacao = new AtualizarVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = versaoCriada.CoVersaoSistemaOperacional,
                CoPlataforma = "A",
                NuVersaoSistemaOperacional = 13.0m,
                NuVersaoSdk = 32.0m
            };
            var versaoAtualizada = await _repository.AtualizarVersaoSistemaOperacionalAsync(
                versaoCriada.CoVersaoSistemaOperacional, entradaAtualizacao);
            Assert.Equal(13.0m, versaoAtualizada.NuVersaoSistemaOperacional);

            // Act & Assert - Excluir
            await _repository.ExcluirVersaoSistemaOperacionalAsync(versaoCriada.CoVersaoSistemaOperacional);
            var existeAposExclusao = await _repository.VersaoSistemaOperacionalExisteAsync(versaoCriada.CoVersaoSistemaOperacional);
            Assert.False(existeAposExclusao);
        }

        #endregion
    }
}
