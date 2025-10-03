using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.DTO;
using Microsoft.EntityFrameworkCore;

namespace API_Miniapp_Gestao.Repositories
{
    public class MiniAppVersaoRepository : IMiniAppVersaoRepository
    {
        private readonly DbEscrita _dbEscrita;
        private readonly DbLeitura _dbLeitura;
        private readonly ILogger<MiniAppVersaoRepository> _logger;

        public MiniAppVersaoRepository(DbEscrita dbEscrita, DbLeitura dbLeitura, ILogger<MiniAppVersaoRepository> logger)
        {
            _dbEscrita = dbEscrita;
            _dbLeitura = dbLeitura;
            _logger = logger;
        }

        public async Task<AtualizarVersaoMiniappDto> AtualizarVersaoMiniappAsync(Guid coVersao, AtualizarVersaoMiniappDto entrada)
        {
            var versao = await _dbEscrita.Nbmtb006VersaoMiniapps.FindAsync(coVersao);
            if (versao == null)
                throw new KeyNotFoundException("Versão não encontrada");

            if (entrada.NuVersao.HasValue)
                versao.NuVersaoMiniapp = entrada.NuVersao.Value;
                
            if (entrada.PcExpansao.HasValue)
                versao.PcExpansaoMiniapp = entrada.PcExpansao.Value;
                
            versao.IcAtivo = entrada.IcAtivo;
            
            if (!string.IsNullOrWhiteSpace(entrada.EdVersaoMiniapp))
                versao.EdVersaoMiniapp = entrada.EdVersaoMiniapp;

            await _dbEscrita.SaveChangesAsync();

            // Retornar os valores atualizados da base de dados
            return new AtualizarVersaoMiniappDto
            {
                CoVersao = versao.CoVersaoMiniapp,
                CoMiniapp = versao.CoMiniapp,
                NuVersao = versao.NuVersaoMiniapp,
                PcExpansao = versao.PcExpansaoMiniapp,
                IcAtivo = versao.IcAtivo,
                EdVersaoMiniapp = versao.EdVersaoMiniapp
            };
        }

        public async Task<RetornoCriarVersaoMiniappDto> CriarVersaoMiniappAsync(EntradaCriarVersaoMiniappDto entrada)
        {
            _logger.LogInformation("Iniciando criação de versão para o miniapp: {CoMiniapp}", entrada.CoMiniapp);

            // Verificar se o miniapp existe
            var miniappExiste = await _dbLeitura.Nbmtb004Miniapps
                .AnyAsync(m => m.CoMiniapp == entrada.CoMiniapp);

            if (!miniappExiste)
            {
                _logger.LogWarning("Miniapp com CoMiniapp {CoMiniapp} não encontrado.", entrada.CoMiniapp);
                throw new KeyNotFoundException($"Miniapp com CoMiniapp {entrada.CoMiniapp} não encontrado.");
            }

            // Processar versão numérica
            decimal versaoNumerica = entrada.NuVersao ?? 1.0m;

            // Processar expansão
            decimal expansaoNumerica = entrada.PcExpansao ?? 0.0m;

            // Criar nova versão
            var novaVersao = new Nbmtb006VersaoMiniapp
            {
                CoVersaoMiniapp = Guid.NewGuid(),
                CoMiniapp = entrada.CoMiniapp,
                NuVersaoMiniapp = versaoNumerica,
                PcExpansaoMiniapp = expansaoNumerica,
                IcAtivo = entrada.IcAtivo,
                DhInicioVigencia = DateTime.UtcNow,
                EdVersaoMiniapp = !string.IsNullOrEmpty(entrada.EdVersaoMiniapp) ? entrada.EdVersaoMiniapp : $"https://releases.example.com/miniapp/{entrada.CoMiniapp}/v{versaoNumerica.ToString("G", System.Globalization.CultureInfo.InvariantCulture)}"
            };

            try
            {
                _dbEscrita.Nbmtb006VersaoMiniapps.Add(novaVersao);
                await _dbEscrita.SaveChangesAsync();

                _logger.LogInformation("Versão do miniapp criada com sucesso. CoVersao: {CoVersao}", novaVersao.CoVersaoMiniapp);

                return new RetornoCriarVersaoMiniappDto
                {
                    CoVersao = novaVersao.CoVersaoMiniapp,
                    CoMiniapp = novaVersao.CoMiniapp,
                    NuVersao = novaVersao.NuVersaoMiniapp,
                    PcExpansao = novaVersao.PcExpansaoMiniapp,
                    IcAtivo = novaVersao.IcAtivo,
                    EdVersaoMiniapp = novaVersao.EdVersaoMiniapp
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar versão do miniapp: {CoMiniapp}", entrada.CoMiniapp);
                throw new InvalidOperationException($"Falha ao criar versão para o miniapp {entrada.CoMiniapp}", ex);
            }
        }

        public async Task<List<VersaoMiniappDto>> GetVersoesByMiniappAsync(Guid coMiniapp)
        {
            _logger.LogInformation("Iniciando busca de versões para o miniapp: {CoMiniapp}", coMiniapp);

            // Verificar se o miniapp existe
            var miniappExiste = await _dbLeitura.Nbmtb004Miniapps
                .AnyAsync(m => m.CoMiniapp == coMiniapp);

            if (!miniappExiste)
            {
                _logger.LogWarning("Miniapp com código {CoMiniapp} não encontrado.", coMiniapp);
                throw new KeyNotFoundException($"Miniapp com código {coMiniapp} não encontrado.");
            }

            try
            {
                var versoes = await _dbLeitura.Nbmtb006VersaoMiniapps
                    .Where(v => v.CoMiniapp == coMiniapp)
                    .OrderByDescending(v => v.DhInicioVigencia)
                    .Select(v => new VersaoMiniappDto
                    {
                        CoVersao = v.CoVersaoMiniapp,
                        NuVersao = v.NuVersaoMiniapp,
                        PcExpansao = v.PcExpansaoMiniapp,
                        IcAtivo = v.IcAtivo,
                        EdVersaoMiniapp = v.EdVersaoMiniapp
                    })
                    .ToListAsync();

                _logger.LogInformation("Busca de versões concluída. Total encontrado: {Count}", versoes.Count);

                return versoes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar versões do miniapp: {CoMiniapp}", coMiniapp);
                throw new InvalidOperationException($"Falha ao buscar versões para o miniapp {coMiniapp}", ex);
            }
        }

        public async Task<VersaoMiniappDto?> GetVersaoByIdAsync(Guid coVersao)
        {
            var versao = await _dbLeitura.Nbmtb006VersaoMiniapps.FirstOrDefaultAsync(v => v.CoVersaoMiniapp == coVersao);
            if (versao == null) return null;
            return new VersaoMiniappDto
            {
                CoVersao = versao.CoVersaoMiniapp,
                NuVersao = versao.NuVersaoMiniapp,
                PcExpansao = versao.PcExpansaoMiniapp,
                IcAtivo = versao.IcAtivo,
                EdVersaoMiniapp = versao.EdVersaoMiniapp
            };
        }

        public async Task<bool> ExisteVersaoDuplicadaAsync(Guid coMiniapp, decimal? nuVersao, Guid coVersaoAtual)
        {
            return await _dbLeitura.Nbmtb006VersaoMiniapps.AnyAsync(v => v.CoMiniapp == coMiniapp && v.NuVersaoMiniapp == nuVersao && v.CoVersaoMiniapp != coVersaoAtual);
        }

        public async Task<bool> MiniappAtivoAsync(Guid coMiniapp)
        {
            var miniapp = await _dbLeitura.Nbmtb004Miniapps.FirstOrDefaultAsync(m => m.CoMiniapp == coMiniapp);
            return miniapp != null && miniapp.IcAtivo;
        }

        public async Task<VersaoMiniappDto?> GetVersaoByMiniappENumeroAsync(Guid coMiniapp, decimal nuVersao)
        {
            var versao = await _dbLeitura.Nbmtb006VersaoMiniapps
                .FirstOrDefaultAsync(v => v.CoMiniapp == coMiniapp && v.NuVersaoMiniapp == nuVersao);

            if (versao == null)
                return null;

            return new VersaoMiniappDto
            {
                CoVersao = versao.CoVersaoMiniapp,
                NuVersao = versao.NuVersaoMiniapp,
                PcExpansao = versao.PcExpansaoMiniapp,
                IcAtivo = versao.IcAtivo
            };
        }
    }
}
