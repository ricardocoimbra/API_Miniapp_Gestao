using Microsoft.EntityFrameworkCore;
using API_Miniapp_Gestao.Models;

namespace API_Miniapp_Gestao.Tests.Base
{
    public class TestBase : IDisposable
    {
        protected DbEscrita DbEscrita { get; private set; }
        protected DbLeitura DbLeitura { get; private set; }

        public TestBase()
        {
            // Configurar bancos em memória compartilhado para simular ambiente real
            var nomeDb = $"TestDb_{Guid.NewGuid()}";

            var optionsEscrita = new DbContextOptionsBuilder<DbEscrita>()
                .UseInMemoryDatabase(databaseName: nomeDb)
                .Options;

            var optionsLeitura = new DbContextOptionsBuilder<DbLeitura>()
                .UseInMemoryDatabase(databaseName: nomeDb)
                .Options;

            DbEscrita = new DbEscrita(optionsEscrita);
            DbLeitura = new DbLeitura(optionsLeitura);
        }

        protected async Task<Guid> CriarMiniappParaTeste(
            string noMiniapp = "Miniapp Teste",
            string noApelidoMiniapp = "Teste",
            string deMiniapp = "Descrição do teste",
            bool icMiniappInicial = false,
            bool icAtivo = true)
        {
            var coMiniapp = Guid.NewGuid();

            var miniapp = new Nbmtb004Miniapp
            {
                CoMiniapp = coMiniapp,
                NoMiniapp = noMiniapp,
                NoApelidoMiniapp = noApelidoMiniapp,
                DeMiniapp = deMiniapp,
                IcMiniappInicial = icMiniappInicial,
                IcAtivo = icAtivo
            };

            DbEscrita.Nbmtb004Miniapps.Add(miniapp);
            await DbEscrita.SaveChangesAsync();

            return coMiniapp;
        }

        protected async Task<Guid> CriarVersaoParaTeste(
            Guid coMiniapp,
            decimal nuVersao = 1.0m,
            decimal pcExpansao = 0.0m,
            bool icAtivo = true,
            string? edVersaoMiniapp = null)
        {
            var coVersao = Guid.NewGuid();
            edVersaoMiniapp ??= nuVersao.ToString();

            var versao = new Nbmtb006VersaoMiniapp
            {
                CoVersaoMiniapp = coVersao,
                CoMiniapp = coMiniapp,
                NuVersaoMiniapp = nuVersao,
                PcExpansaoMiniapp = pcExpansao,
                IcAtivo = icAtivo,
                DhInicioVigencia = DateTime.UtcNow,
                EdVersaoMiniapp = edVersaoMiniapp
            };

            DbEscrita.Nbmtb006VersaoMiniapps.Add(versao);
            await DbEscrita.SaveChangesAsync();

            return coVersao;
        }

        public void Dispose()
        {
            DbEscrita?.Dispose();
            DbLeitura?.Dispose();
        }
    }
}
