using Microsoft.Extensions.Logging;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services.Interfaces;

namespace API_Miniapp_Gestao.Services
{
    public class VersaoSistemaOperacionalMiniappService : IVersaoSistemaOperacionalMiniappService
    {
        private readonly IVersaoSistemaOperacionalMiniappRepository _repository;
        private readonly ILogger<VersaoSistemaOperacionalMiniappService> _logger;

        public VersaoSistemaOperacionalMiniappService(
            IVersaoSistemaOperacionalMiniappRepository repository,
            ILogger<VersaoSistemaOperacionalMiniappService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<RetornoVersaoSistemaOperacionalMiniappDto> CriarVersaoSistemaOperacionalAsync(EntradaVersaoSistemaOperacionalMiniappDto entrada)
        {
            ArgumentNullException.ThrowIfNull(entrada);

            var resultado = await _repository.CriarVersaoSistemaOperacionalAsync(entrada);
            return resultado;
        }

        public async Task<List<ListaRetornoVersaoSistemaOperacionalMiniappDto>> ListarVersoesSistemaOperacionalAsync(ListaVersaoSistemaOperacionalMiniappDto entrada)
        {
            ArgumentNullException.ThrowIfNull(entrada);

            var versoes = await _repository.ListarVersoesSistemaOperacionalAsync(entrada);

            if (versoes == null || !versoes.Any())
            {
                _logger.LogInformation("Nenhuma versão do sistema operacional encontrada para os critérios fornecidos");
                throw new BusinessException("NENHUMA_VERSAO_ENCONTRADA", "Nenhuma versão do sistema operacional foi encontrada", 404);
            }
            else
            {
                return versoes;
            }
        }

        public async Task<RetornoVersaoSistemaOperacionalMiniappDto> AtualizarVersaoSistemaOperacionalAsync(AtualizarVersaoSistemaOperacionalMiniappDto entrada)
        {
            ArgumentNullException.ThrowIfNull(entrada);

            return await _repository.AtualizarVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional, entrada);

        }

        public async Task ExcluirVersaoSistemaOperacionalAsync(RemoveVersaoSistemaOperacionalMiniappDto entrada)
        {
            ArgumentNullException.ThrowIfNull(entrada);

            // Verificar se a versão existe antes de tentar excluir
            var existe = await _repository.VersaoSistemaOperacionalExisteAsync(entrada.CoVersaoSistemaOperacional);
            if (!existe)
            {
                _logger.LogWarning("Versão do sistema operacional não encontrada: {CoVersaoSistemaOperacional}", entrada.CoVersaoSistemaOperacional);
                throw new BusinessException("VERSAO_NAO_ENCONTRADA", $"Versão do sistema operacional com ID {entrada.CoVersaoSistemaOperacional} não foi encontrada", 404);
            }

            await _repository.ExcluirVersaoSistemaOperacionalAsync(entrada.CoVersaoSistemaOperacional);

        }
    }
}
