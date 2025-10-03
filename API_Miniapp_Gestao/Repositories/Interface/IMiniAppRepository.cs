using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Repositories.Interface
{
    public interface IMiniAppRepository
    {
        Task<List<MiniappDto>> GetMiniAppsAsync();
        Task<MiniappDto> GetMiniAppByCoMiniappAsync(Guid coMiniapp);
        Task<MiniappDto> GetMiniAppByNameAsync(string noMiniapp);
        Task CreateMiniAppAsync(MiniappDto novoMiniapp);
        Task<List<VersaoMiniappDto>> GetVersoesMiniappPorCoMiniappAsync(Guid coMiniapp);
    }
}