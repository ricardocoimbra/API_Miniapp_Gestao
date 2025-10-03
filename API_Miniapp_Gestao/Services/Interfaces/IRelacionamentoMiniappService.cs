using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Services.Interfaces
{
    public interface IRelacionamentoMiniappService
    {
        Task CriarRelacionamentoAsync(IncluirRelacionamentoMiniappDto relacionamento);
        Task<ListaRelacionamentosDto> ListarRelacionamentosAsync(EntradaRelacionamentosDto entrada);
        Task ExcluirRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho);
        Task<List<RetornoRelacionamentosDto>> ListarTodosRelacionamentosAsync();
        Task<bool> ValidarRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho);
        Task EditarRelacionamentoAsync(EditarRelacionamentoMiniappDto entrada);
        Task<bool> RelacionamentoExisteAsync(Guid coMiniappPai, Guid coMiniappFilho);
    }
}