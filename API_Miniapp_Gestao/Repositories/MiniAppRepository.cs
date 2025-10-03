using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.DTO;
using Microsoft.EntityFrameworkCore;
using API_Miniapp_Gestao.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Repositories;

[ExcludeFromCodeCoverage]
public class MiniAppRepository : IMiniAppRepository
{
    private readonly DbEscrita _dbEscrita;
    private readonly DbLeitura _dbLeitura;
    private readonly ILogger<MiniAppRepository> _logger;

    public MiniAppRepository(DbEscrita dbEscrita, DbLeitura dbLeitura, ILogger<MiniAppRepository> logger)
    {
        _dbEscrita = dbEscrita;
        _dbLeitura = dbLeitura;
        _logger = logger;
    }

    public async Task<List<MiniappDto>> GetMiniAppsAsync()
    {
        var todosOsMiniApps = await _dbLeitura.Nbmtb004Miniapps
                .Select(m => new MiniappDto
                {
                    CoMiniapp = m.CoMiniapp,
                    NoMiniapp = m.NoMiniapp,
                    NoApelidoMiniapp = m.NoApelidoMiniapp,
                    DeMiniapp = m.DeMiniapp,
                    IcMiniappInicial = m.IcMiniappInicial,
                    IcAtivo = m.IcAtivo
                })
                .ToListAsync();
        return todosOsMiniApps;

    }

    public async Task<MiniappDto> GetMiniAppByCoMiniappAsync(Guid coMiniapp)
    {
        var miniApp = await _dbLeitura.Nbmtb004Miniapps
            .Where(m => m.CoMiniapp == coMiniapp)
            .Select(m => new MiniappDto
            {
                CoMiniapp = m.CoMiniapp,
                NoMiniapp = m.NoMiniapp,
                NoApelidoMiniapp = m.NoApelidoMiniapp,
                DeMiniapp = m.DeMiniapp,
                IcMiniappInicial = m.IcMiniappInicial,
                IcAtivo = m.IcAtivo
            })
            .FirstOrDefaultAsync();

        return miniApp!;
    }

    public async Task<MiniappDto> GetMiniAppByNameAsync(string noMiniapp)
    {
        var miniApp = await _dbLeitura.Nbmtb004Miniapps
            .Where(m => m.NoMiniapp == noMiniapp)
            .Select(m => new MiniappDto
            {
                CoMiniapp = m.CoMiniapp,
                NoMiniapp = m.NoMiniapp,
                NoApelidoMiniapp = m.NoApelidoMiniapp,
                DeMiniapp = m.DeMiniapp,
                IcMiniappInicial = m.IcMiniappInicial,
                IcAtivo = m.IcAtivo
            })
            .FirstOrDefaultAsync();

        return miniApp!;
    }

    public async Task CreateMiniAppAsync(MiniappDto novoMiniapp)
    {
        var entidadeMiniapp = new Nbmtb004Miniapp
        {
            CoMiniapp = novoMiniapp.CoMiniapp,
            NoMiniapp = novoMiniapp.NoMiniapp!,
            NoApelidoMiniapp = novoMiniapp.NoApelidoMiniapp!,
            DeMiniapp = novoMiniapp.DeMiniapp,
            IcMiniappInicial = novoMiniapp.IcMiniappInicial,
            IcAtivo = novoMiniapp.IcAtivo
        };
        try
        {
            _dbEscrita.Nbmtb004Miniapps.Add(entidadeMiniapp);
            await _dbEscrita.SaveChangesAsync();
            _logger.LogInformation("Miniapp criado com sucesso - ID: {CoMiniapp}, Nome: {NoMiniapp}",
                entidadeMiniapp.CoMiniapp, entidadeMiniapp.NoMiniapp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar novo Miniapp no banco de dados.");
            throw new BusinessException("Erro de acesso ao Banco de Dados", "Erro ao criar novo Miniapp", 500);
        }
    }

    public async Task<List<VersaoMiniappDto>> GetVersoesMiniappPorCoMiniappAsync(Guid coMiniapp)
    {
        var versoes = await _dbLeitura.Nbmtb006VersaoMiniapps
            .Where(v => v.CoMiniapp == coMiniapp)
            .Select(v => new VersaoMiniappDto
            {
                CoVersao = v.CoVersaoMiniapp,
                NuVersao = v.NuVersaoMiniapp,
                PcExpansao = v.PcExpansaoMiniapp,
                IcAtivo = v.IcAtivo,
                EdVersaoMiniapp = v.EdVersaoMiniapp
            })
            .ToListAsync();

        return versoes;
    }
}