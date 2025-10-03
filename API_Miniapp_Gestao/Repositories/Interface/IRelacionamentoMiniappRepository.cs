using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Repositories.Interface
{
    public interface IRelacionamentoMiniappRepository
    {
        Task<bool> VerificarRelacionamentoExisteAsync(Guid coMiniappPai, Guid coMiniappFilho);
        Task CriarRelacionamentoAsync(IncluirRelacionamentoMiniappDto relacionamento);
        Task<List<RetornoRelacionamentosDto>> GetMiniappsPaisAsync(Guid coMiniappFilho);
        Task<List<RetornoRelacionamentosDto>> GetMiniappsFilhosAsync(Guid coMiniappPai);
        Task<bool> MiniappExisteAsync(Guid coMiniapp);
        Task ExcluirRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho);
        Task<List<RetornoRelacionamentosDto>> GetTodosRelacionamentosAsync();
    }
}
