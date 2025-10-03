using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services.Interfaces;

namespace API_Miniapp_Gestao.Services
{
    public class MiniappVersaoService : IMiniappVersaoService
    {
        private readonly IMiniAppVersaoRepository _miniAppVersaoRepository;
        private readonly ILogger<MiniappVersaoService> _logger;

        public MiniappVersaoService(IMiniAppVersaoRepository miniAppVersaoRepository, ILogger<MiniappVersaoService> logger)
        {
            _miniAppVersaoRepository = miniAppVersaoRepository;
            _logger = logger;
        }

        
        public async Task<RetornoCriarVersaoMiniappDto> CriarVersaoMiniappAsync(EntradaCriarVersaoMiniappDto entrada)
        {
            _logger.LogInformation("Iniciando criação de nova versão para o miniapp: {CoMiniapp}", entrada.CoMiniapp);
            
            try
            {
                // Delega a criação para o repository
                var resultado = await _miniAppVersaoRepository.CriarVersaoMiniappAsync(entrada);
                
                _logger.LogInformation("Versão do miniapp criada com sucesso. CoVersao: {CoVersao}", resultado.CoVersao);
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar versão do miniapp: {CoMiniapp}", entrada.CoMiniapp);
                throw new InvalidOperationException($"Falha ao criar versão para o miniapp {entrada.CoMiniapp}", ex);
            }
        }

        public async Task<RetornoListarVersoesDto> ListarVersoesMiniappAsync(EntradaMiniappDto entrada)
        {
            if (string.IsNullOrEmpty(entrada.CoMiniapp))
            {
                _logger.LogWarning("CoMiniapp não informado para listar versões");
                throw new ArgumentException("CoMiniapp é obrigatório para listar versões");
            }

            if (!Guid.TryParse(entrada.CoMiniapp, out var coMiniappGuid))
            {
                _logger.LogWarning("CoMiniapp inválido: {CoMiniapp}", entrada.CoMiniapp);
                throw new ArgumentException("CoMiniapp deve ser um GUID válido");
            }

            _logger.LogInformation("Iniciando listagem de versões para o miniapp: {CoMiniapp}", coMiniappGuid);

            try
            {
                var versoes = await _miniAppVersaoRepository.GetVersoesByMiniappAsync(coMiniappGuid);

                _logger.LogInformation("Listagem de versões concluída. Total encontrado: {Count}", versoes.Count);

                var retornoListaVersoes = versoes.Select(v => new VersaoMiniappDto
                {
                    CoVersao = v.CoVersao,
                    NuVersao = v.NuVersao,
                    PcExpansao = v.PcExpansao,
                    IcAtivo = v.IcAtivo,
                    EdVersaoMiniapp = v.EdVersaoMiniapp
                }).ToList();

                return new RetornoListarVersoesDto
                {
                    RetornoListaVersoes = retornoListaVersoes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar versões do miniapp: {CoMiniapp}", coMiniappGuid);
                throw new InvalidOperationException($"Falha ao listar versões para o miniapp {coMiniappGuid}", ex);
            }
        }

        public async Task<AtualizarVersaoMiniappDto> AtualizarVersaoMiniappAsync(EntradaAtualizarVersaoMiniappDto entrada)
        {
            if (entrada == null)
            {
                _logger.LogWarning("Entrada não informada para atualizar versão");
                throw new ArgumentException("Dados de entrada são obrigatórios para atualizar versão");
            }

            if (entrada.CoVersao == Guid.Empty)
            {
                _logger.LogWarning("CoVersao não informado para atualizar versão");
                throw new ArgumentException("CoVersao é obrigatório para atualizar versão");
            }

            if (entrada.CoMiniapp == Guid.Empty)
            {
                _logger.LogWarning("CoMiniapp não informado para atualizar versão");
                throw new ArgumentException("CoMiniapp é obrigatório para atualizar versão");
            }

            _logger.LogInformation("Iniciando atualização de versão {CoVersao} para o miniapp: {CoMiniapp}", entrada.CoVersao, entrada.CoMiniapp);

            try
            {
                // Verificar se a versão existe
                var versaoExistente = await _miniAppVersaoRepository.GetVersaoByIdAsync(entrada.CoVersao);
                if (versaoExistente == null)
                {
                    _logger.LogWarning("Versão não encontrada: {CoVersao}", entrada.CoVersao);
                    throw new KeyNotFoundException($"Versão com ID {entrada.CoVersao} não encontrada");
                }

                // Verificar se o miniapp está ativo
                var miniappAtivo = await _miniAppVersaoRepository.MiniappAtivoAsync(entrada.CoMiniapp);
                if (!miniappAtivo)
                {
                    _logger.LogWarning("Miniapp inativo: {CoMiniapp}", entrada.CoMiniapp);
                    throw new ArgumentException("Não é possível atualizar versões de um miniapp inativo");
                }

                // Verificar duplicidade de versão (se nuVersao foi alterado)
                if (entrada.NuVersao.HasValue && entrada.NuVersao != versaoExistente.NuVersao)
                {
                    var versaoDuplicada = await _miniAppVersaoRepository.ExisteVersaoDuplicadaAsync(entrada.CoMiniapp, entrada.NuVersao, entrada.CoVersao);
                    if (versaoDuplicada)
                    {
                        _logger.LogWarning("Versão duplicada: {NuVersao} para miniapp: {CoMiniapp}", entrada.NuVersao, entrada.CoMiniapp);
                        throw new ArgumentException($"Já existe uma versão {entrada.NuVersao} para este miniapp");
                    }
                }

                // Preparar DTO para atualização
                var dtoParaAtualizacao = new AtualizarVersaoMiniappDto
                {
                    CoVersao = entrada.CoVersao,
                    CoMiniapp = entrada.CoMiniapp,
                    NuVersao = entrada.NuVersao,
                    PcExpansao = entrada.PcExpansao,
                    IcAtivo = entrada.IcAtivo,
                    EdVersaoMiniapp = entrada.EdVersaoMiniapp
                };

                // Atualizar no repositório
                var resultado = await _miniAppVersaoRepository.AtualizarVersaoMiniappAsync(entrada.CoVersao, dtoParaAtualizacao);

                _logger.LogInformation("Atualização de versão concluída para a versão: {CoVersao}", entrada.CoVersao);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar versão {CoVersao} do miniapp: {CoMiniapp}", entrada.CoVersao, entrada.CoMiniapp);
                throw new InvalidOperationException($"Falha ao atualizar versão {entrada.CoVersao} do miniapp {entrada.CoMiniapp}", ex);
            }
        }
    }
}
