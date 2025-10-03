using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Services.Interfaces
{
    public interface IMiniappVersaoService
    {
        Task<RetornoCriarVersaoMiniappDto> CriarVersaoMiniappAsync(EntradaCriarVersaoMiniappDto entrada);
        Task<RetornoListarVersoesDto> ListarVersoesMiniappAsync(EntradaMiniappDto entrada);
        Task<AtualizarVersaoMiniappDto> AtualizarVersaoMiniappAsync(EntradaAtualizarVersaoMiniappDto entrada);
    }
}
