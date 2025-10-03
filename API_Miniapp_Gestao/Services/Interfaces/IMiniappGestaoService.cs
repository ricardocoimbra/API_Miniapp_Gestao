using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Services.Interfaces
{
    public interface IMiniAppService
    {
        Task<List<RetornoMiniappDto>> ConsultaMiniapps(EntradaMiniappDto entrada);
        Task<MiniappDto> CriarMiniapp(CriarMiniappDto entrada);
        Task<bool> MiniappExists(string noMiniapp);
    }
}