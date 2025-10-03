using Microsoft.EntityFrameworkCore;
using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.Repositories.Interface;

namespace API_Miniapp_Gestao.Repositories
{
    public class VersaoSistemaOperacionalMiniappRepository : IVersaoSistemaOperacionalMiniappRepository
    {
        private readonly DbEscrita _context;

        // Constantes para evitar duplicação de strings
        private const string VERSAO_NAO_ENCONTRADA_MSG = "Versão do sistema operacional com ID {0} não encontrada";
        private const string VERSAO_DUPLICADA_MSG = "Já existe uma versão {0} para a plataforma {1}";

        public VersaoSistemaOperacionalMiniappRepository(DbEscrita context)
        {
            _context = context;
        }

        public async Task<RetornoVersaoSistemaOperacionalMiniappDto> CriarVersaoSistemaOperacionalAsync(EntradaVersaoSistemaOperacionalMiniappDto entrada)
        {
            // Verificar se já existe uma versão com a mesma plataforma e número de versão
            var existeDuplicacao = await VerificarDuplicacaoAsync(entrada.CoPlataforma, entrada.NuVersaoSistemaOperacional);
            if (existeDuplicacao)
            {
                throw new InvalidOperationException(string.Format(VERSAO_DUPLICADA_MSG, entrada.NuVersaoSistemaOperacional, entrada.CoPlataforma));
            }

            // Gerar um novo GUID ou usar o fornecido (se fornecido)
            var coVersaoSistemaOperacional = entrada.CoVersaoSistemaOperacional ?? Guid.NewGuid();

            var novaVersao = new Nbmtb001VersaoSistemaOperacional
            {
                CoVersaoSistemaOperacional = coVersaoSistemaOperacional,
                CoPlataforma = entrada.CoPlataforma,
                NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional,
                NuVersaoSdk = entrada.NuVersaoSdk
            };

            _context.Nbmtb001VersaoSistemaOperacionals.Add(novaVersao);
            await _context.SaveChangesAsync();

            return CriarRetornoVersaoSistemaOperacional(novaVersao);
        }

        public async Task<List<ListaRetornoVersaoSistemaOperacionalMiniappDto>> ListarVersoesSistemaOperacionalAsync(ListaVersaoSistemaOperacionalMiniappDto entrada)
        {
            var query = _context.Nbmtb001VersaoSistemaOperacionals.AsQueryable();

            // Aplicar filtros opcionais conforme fornecidos
            if (entrada.CoVersaoSistemaOperacional.HasValue)
            {
                query = query.Where(v => v.CoVersaoSistemaOperacional == entrada.CoVersaoSistemaOperacional.Value);
            }

            if (!string.IsNullOrEmpty(entrada.CoPlataforma))
            {
                query = query.Where(v => v.CoPlataforma == entrada.CoPlataforma);
            }

            if (entrada.NuVersaoSistemaOperacional.HasValue)
            {
                query = query.Where(v => v.NuVersaoSistemaOperacional == entrada.NuVersaoSistemaOperacional.Value);
            }

            if (entrada.NuVersaoSdk.HasValue)
            {
                query = query.Where(v => v.NuVersaoSdk == entrada.NuVersaoSdk.Value);
            }

            var versoes = await query
                .OrderBy(v => v.CoPlataforma)
                .ThenBy(v => v.NuVersaoSistemaOperacional)
                .ToListAsync();

            return versoes.Select(v => new ListaRetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = v.CoVersaoSistemaOperacional,
                CoPlataforma = v.CoPlataforma,
                NuVersaoSistemaOperacional = v.NuVersaoSistemaOperacional,
                NuVersaoSdk = v.NuVersaoSdk
            }).ToList();
        }

        public async Task<RetornoVersaoSistemaOperacionalMiniappDto> AtualizarVersaoSistemaOperacionalAsync(Guid coVersaoSistemaOperacional, AtualizarVersaoSistemaOperacionalMiniappDto entrada)
        {
            var versaoExistente = await ObterVersaoExistenteAsync(coVersaoSistemaOperacional);
            await ValidarDuplicacaoParaAtualizacaoAsync(entrada, versaoExistente, coVersaoSistemaOperacional);
            AtualizarCamposVersao(entrada, versaoExistente);
            await _context.SaveChangesAsync();

            return CriarRetornoVersaoSistemaOperacional(versaoExistente);
        }

        public async Task ExcluirVersaoSistemaOperacionalAsync(Guid coVersaoSistemaOperacional)
        {
            var versao = await _context.Nbmtb001VersaoSistemaOperacionals
                .FirstOrDefaultAsync(v => v.CoVersaoSistemaOperacional == coVersaoSistemaOperacional);

            if (versao == null)
            {
                throw new KeyNotFoundException(string.Format(VERSAO_NAO_ENCONTRADA_MSG, coVersaoSistemaOperacional));
            }

            _context.Nbmtb001VersaoSistemaOperacionals.Remove(versao);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VersaoSistemaOperacionalExisteAsync(Guid coVersaoSistemaOperacional)
        {
            return await _context.Nbmtb001VersaoSistemaOperacionals
                .AnyAsync(v => v.CoVersaoSistemaOperacional == coVersaoSistemaOperacional);
        }

        public async Task<bool> VerificarDuplicacaoAsync(string coPlataforma, decimal nuVersaoSistemaOperacional, Guid? coVersaoSistemaOperacionalAtual = null)
        {
            var query = _context.Nbmtb001VersaoSistemaOperacionals
                .Where(v => v.CoPlataforma == coPlataforma && v.NuVersaoSistemaOperacional == nuVersaoSistemaOperacional);

            if (coVersaoSistemaOperacionalAtual.HasValue)
            {
                query = query.Where(v => v.CoVersaoSistemaOperacional != coVersaoSistemaOperacionalAtual.Value);
            }

            return await query.AnyAsync();
        }

        #region Métodos Privados

        private async Task<Nbmtb001VersaoSistemaOperacional> ObterVersaoExistenteAsync(Guid coVersaoSistemaOperacional)
        {
            var versaoExistente = await _context.Nbmtb001VersaoSistemaOperacionals
                .FirstOrDefaultAsync(v => v.CoVersaoSistemaOperacional == coVersaoSistemaOperacional);

            if (versaoExistente == null)
            {
                throw new KeyNotFoundException(string.Format(VERSAO_NAO_ENCONTRADA_MSG, coVersaoSistemaOperacional));
            }

            return versaoExistente;
        }

        private async Task ValidarDuplicacaoParaAtualizacaoAsync(
            AtualizarVersaoSistemaOperacionalMiniappDto entrada,
            Nbmtb001VersaoSistemaOperacional versaoExistente,
            Guid coVersaoSistemaOperacional)
        {
            var plataformaMudou = entrada.CoPlataforma != versaoExistente.CoPlataforma;
            var versaoMudou = entrada.NuVersaoSistemaOperacional != versaoExistente.NuVersaoSistemaOperacional;

            if (plataformaMudou || versaoMudou)
            {
                var existeDuplicacao = await VerificarDuplicacaoAsync(entrada.CoPlataforma, entrada.NuVersaoSistemaOperacional, coVersaoSistemaOperacional);
                if (existeDuplicacao)
                {
                    throw new InvalidOperationException(string.Format(VERSAO_DUPLICADA_MSG, entrada.NuVersaoSistemaOperacional, entrada.CoPlataforma));
                }
            }
        }

        private static void AtualizarCamposVersao(AtualizarVersaoSistemaOperacionalMiniappDto entrada, Nbmtb001VersaoSistemaOperacional versaoExistente)
        {
            versaoExistente.CoPlataforma = entrada.CoPlataforma;
            versaoExistente.NuVersaoSistemaOperacional = entrada.NuVersaoSistemaOperacional;
            versaoExistente.NuVersaoSdk = entrada.NuVersaoSdk;
        }

        private static RetornoVersaoSistemaOperacionalMiniappDto CriarRetornoVersaoSistemaOperacional(Nbmtb001VersaoSistemaOperacional versao)
        {
            return new RetornoVersaoSistemaOperacionalMiniappDto
            {
                CoVersaoSistemaOperacional = versao.CoVersaoSistemaOperacional,
                CoPlataforma = versao.CoPlataforma,
                NuVersaoSistemaOperacional = versao.NuVersaoSistemaOperacional,
                NuVersaoSdk = versao.NuVersaoSdk
            };
        }

        #endregion
    }
}
