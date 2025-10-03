using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services.Interfaces;

namespace API_Miniapp_Gestao.Services
{
    public class MiniAppService : IMiniAppService
    {
        private readonly IMiniAppRepository _miniAppRepository;
        private readonly IMiniAppVersaoRepository _miniAppVersaoRepository;
        private readonly ILogger<MiniAppService> _logger;

        public MiniAppService(IMiniAppRepository miniAppRepository, IMiniAppVersaoRepository miniAppVersaoRepository, ILogger<MiniAppService> logger)
        {
            _miniAppRepository = miniAppRepository;
            _miniAppVersaoRepository = miniAppVersaoRepository;
            _logger = logger;
        }

        public async Task<List<RetornoMiniappDto>> ConsultaMiniapps(EntradaMiniappDto entrada)
        {
            var listaMiniapps = new List<RetornoMiniappDto>();

            if (string.IsNullOrEmpty(entrada.CoMiniapp))
            {
                try
                {
                    var miniapps = await _miniAppRepository.GetMiniAppsAsync();

                    foreach (var m in miniapps)
                    {
                        var listaVersoesMiniapps = await _miniAppVersaoRepository.GetVersoesByMiniappAsync(m.CoMiniapp);
                        listaMiniapps.Add(
                            new RetornoMiniappDto()
                            {
                                Miniapp = m,
                                VersoesMiniapp = listaVersoesMiniapps
                            }
                        );
                    }
                    return listaMiniapps;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao listar Miniapps");
                    throw new BusinessException("Listar Miniapps", "Erro ao listar Miniapps", 500);
                }

            }
            else
            {
                // Validar se o coMiniapp é um Guid válido
                if (string.IsNullOrWhiteSpace(entrada.CoMiniapp) || !Guid.TryParse(entrada.CoMiniapp, out var coMiniappGuid))
                {
                    throw new BusinessException("Consultar Miniapp", "Código do miniapp inválido ou não informado", 400);
                }

                var miniapps = await _miniAppRepository.GetMiniAppByCoMiniappAsync(coMiniappGuid);
                if (miniapps == null)
                {
                    return null!;
                }

                var listaVersoesMiniapps = await _miniAppVersaoRepository.GetVersoesByMiniappAsync(Guid.Parse(entrada.CoMiniapp));
                listaMiniapps.Add(new RetornoMiniappDto()
                {
                    Miniapp = miniapps,
                    VersoesMiniapp = listaVersoesMiniapps
                });
                return listaMiniapps;
            }
        }

        public async Task<MiniappDto> CriarMiniapp(CriarMiniappDto entrada)
        {

            var novoMiniapp = new MiniappDto
            {
                CoMiniapp = Guid.NewGuid(),
                NoMiniapp = entrada.NoMiniapp,
                NoApelidoMiniapp = entrada.NoApelidoMiniapp,
                DeMiniapp = entrada.DeMiniapp,
                IcMiniappInicial = entrada.IcMiniappInicial,
                IcAtivo = entrada.IcAtivo
            };

            try
            {
                await _miniAppRepository.CreateMiniAppAsync(novoMiniapp);
                return novoMiniapp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar Miniapp");
                throw new BusinessException("Criar Miniapps", "Erro ao criar Miniapp", 500);
            }

        }

        public async Task<bool> MiniappExists(string noMiniapp)
        {
            var miniapp = await _miniAppRepository.GetMiniAppByNameAsync(noMiniapp);
            return miniapp != null;
        }

    }
}
