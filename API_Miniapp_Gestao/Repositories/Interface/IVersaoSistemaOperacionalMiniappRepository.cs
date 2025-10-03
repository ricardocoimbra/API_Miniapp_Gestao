using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Repositories.Interface
{
    public interface IVersaoSistemaOperacionalMiniappRepository
    {
        Task<RetornoVersaoSistemaOperacionalMiniappDto> CriarVersaoSistemaOperacionalAsync(EntradaVersaoSistemaOperacionalMiniappDto entrada);
        Task<List<ListaRetornoVersaoSistemaOperacionalMiniappDto>> ListarVersoesSistemaOperacionalAsync(ListaVersaoSistemaOperacionalMiniappDto entrada);
        Task<RetornoVersaoSistemaOperacionalMiniappDto> AtualizarVersaoSistemaOperacionalAsync(Guid coVersaoSistemaOperacional, AtualizarVersaoSistemaOperacionalMiniappDto entrada);
        Task ExcluirVersaoSistemaOperacionalAsync(Guid coVersaoSistemaOperacional);
        Task<bool> VersaoSistemaOperacionalExisteAsync(Guid coVersaoSistemaOperacional);
        Task<bool> VerificarDuplicacaoAsync(string coPlataforma, decimal nuVersaoSistemaOperacional, Guid? coVersaoSistemaOperacionalAtual = null);
    }
}
