using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Services.Interfaces
{
    public interface IVersaoSistemaOperacionalMiniappService
    {
        Task<RetornoVersaoSistemaOperacionalMiniappDto> CriarVersaoSistemaOperacionalAsync(EntradaVersaoSistemaOperacionalMiniappDto entrada);
        Task<List<ListaRetornoVersaoSistemaOperacionalMiniappDto>> ListarVersoesSistemaOperacionalAsync(ListaVersaoSistemaOperacionalMiniappDto entrada);
        Task<RetornoVersaoSistemaOperacionalMiniappDto> AtualizarVersaoSistemaOperacionalAsync(AtualizarVersaoSistemaOperacionalMiniappDto entrada);
        Task ExcluirVersaoSistemaOperacionalAsync(RemoveVersaoSistemaOperacionalMiniappDto entrada);
    }
}
