using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.DTO;
using Microsoft.EntityFrameworkCore;

namespace API_Miniapp_Gestao.Repositories
{
    public class RelacionamentoMiniappRepository : IRelacionamentoMiniappRepository
    {
        private readonly DbEscrita _dbEscrita;
        private readonly DbLeitura _dbLeitura;
        private readonly ILogger<RelacionamentoMiniappRepository> _logger;

        public RelacionamentoMiniappRepository(DbEscrita dbEscrita, DbLeitura dbLeitura, ILogger<RelacionamentoMiniappRepository> logger)
        {
            _dbEscrita = dbEscrita;
            _dbLeitura = dbLeitura;
            _logger = logger;
        }

        public async Task<bool> VerificarRelacionamentoExisteAsync(Guid coMiniappPai, Guid coMiniappFilho)
        {
            return await _dbLeitura.Nbmtb005RelacionamentoMiniapps
                .AnyAsync(r => r.CoMiniappPai == coMiniappPai && r.CoMiniappFilho == coMiniappFilho);
        }

        public async Task CriarRelacionamentoAsync(IncluirRelacionamentoMiniappDto relacionamento)
        {
            var novoRelacionamento = new Nbmtb005RelacionamentoMiniapp
            {
                CoMiniappPai = relacionamento.CoMiniappPai,
                CoMiniappFilho = relacionamento.CoMiniappFilho
            };

            _dbEscrita.Nbmtb005RelacionamentoMiniapps.Add(novoRelacionamento);
            await _dbEscrita.SaveChangesAsync();
            
            _logger.LogInformation("Relacionamento criado no banco - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}", 
                relacionamento.CoMiniappPai, relacionamento.CoMiniappFilho);
        }

        public async Task<List<RetornoRelacionamentosDto>> GetMiniappsPaisAsync(Guid coMiniappFilho)
        {
            return await _dbLeitura.Nbmtb005RelacionamentoMiniapps
                .Where(r => r.CoMiniappFilho == coMiniappFilho)
                .Join(_dbLeitura.Nbmtb004Miniapps, 
                      r => r.CoMiniappPai, 
                      m => m.CoMiniapp,
                      (r, m) => new RetornoRelacionamentosDto
                      {
                          CoMiniapp = m.CoMiniapp,
                          NoMiniapp = m.NoMiniapp
                      })
                .OrderBy(r => r.NoMiniapp)
                .ToListAsync();
        }

        public async Task<List<RetornoRelacionamentosDto>> GetMiniappsFilhosAsync(Guid coMiniappPai)
        {
            return await _dbLeitura.Nbmtb005RelacionamentoMiniapps
                .Where(r => r.CoMiniappPai == coMiniappPai)
                .Join(_dbLeitura.Nbmtb004Miniapps, 
                      r => r.CoMiniappFilho, 
                      m => m.CoMiniapp,
                      (r, m) => new RetornoRelacionamentosDto
                      {
                          CoMiniapp = m.CoMiniapp,
                          NoMiniapp = m.NoMiniapp
                      })
                .OrderBy(r => r.NoMiniapp)
                .ToListAsync();
        }

        public async Task<bool> MiniappExisteAsync(Guid coMiniapp)
        {
            return await _dbLeitura.Nbmtb004Miniapps
                .AnyAsync(m => m.CoMiniapp == coMiniapp && m.IcAtivo);
        }

        public async Task ExcluirRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho)
        {
            var relacionamento = await _dbEscrita.Nbmtb005RelacionamentoMiniapps
                .FirstOrDefaultAsync(r => r.CoMiniappPai == coMiniappPai && r.CoMiniappFilho == coMiniappFilho);

            if (relacionamento != null)
            {
                _dbEscrita.Nbmtb005RelacionamentoMiniapps.Remove(relacionamento);
                await _dbEscrita.SaveChangesAsync();
                
                _logger.LogInformation("Relacionamento excluído: Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}", 
                    coMiniappPai, coMiniappFilho);
            }
        }

        public async Task<List<RetornoRelacionamentosDto>> GetTodosRelacionamentosAsync()
        {
            return await _dbLeitura.Nbmtb005RelacionamentoMiniapps
                .Join(_dbLeitura.Nbmtb004Miniapps,
                      r => r.CoMiniappPai,
                      m => m.CoMiniapp,
                      (r, pai) => new { Relacionamento = r, Pai = pai })
                .Join(_dbLeitura.Nbmtb004Miniapps,
                      rp => rp.Relacionamento.CoMiniappFilho,
                      filho => filho.CoMiniapp,
                      (rp, filho) => new RetornoRelacionamentosDto
                      {
                          CoMiniapp = rp.Relacionamento.CoMiniappPai,
                          NoMiniapp = $"{rp.Pai.NoMiniapp} → {filho.NoMiniapp}"
                      })
                .OrderBy(r => r.NoMiniapp)
                .ToListAsync();
        }
    }
}
