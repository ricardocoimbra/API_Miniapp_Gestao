using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Repositories.Interface
{
    public interface IMiniAppVersaoRepository
    {
        Task<RetornoCriarVersaoMiniappDto> CriarVersaoMiniappAsync(EntradaCriarVersaoMiniappDto entrada);
        Task<List<VersaoMiniappDto>> GetVersoesByMiniappAsync(Guid coMiniapp);
        Task<AtualizarVersaoMiniappDto> AtualizarVersaoMiniappAsync(Guid coVersao, AtualizarVersaoMiniappDto entrada);
        Task<VersaoMiniappDto?> GetVersaoByIdAsync(Guid coVersao);
        Task<VersaoMiniappDto?> GetVersaoByMiniappENumeroAsync(Guid coMiniapp, decimal nuVersao);
        Task<bool> ExisteVersaoDuplicadaAsync(Guid coMiniapp, decimal? nuVersao, Guid coVersaoAtual);
        Task<bool> MiniappAtivoAsync(Guid coMiniapp);
    }
}
