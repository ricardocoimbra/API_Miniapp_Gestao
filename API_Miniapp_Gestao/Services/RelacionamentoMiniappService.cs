using API_Miniapp_Gestao.DTO;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services.Interfaces;

namespace API_Miniapp_Gestao.Services
{
    public class RelacionamentoMiniappService : IRelacionamentoMiniappService
    {
        private readonly IRelacionamentoMiniappRepository _relacionamentoRepository;
        private readonly ILogger<RelacionamentoMiniappService> _logger;

        public RelacionamentoMiniappService(
            IRelacionamentoMiniappRepository relacionamentoRepository, 
            ILogger<RelacionamentoMiniappService> logger)
        {
            _relacionamentoRepository = relacionamentoRepository;
            _logger = logger;
        }

        public async Task CriarRelacionamentoAsync(IncluirRelacionamentoMiniappDto relacionamento)
        {
            if (relacionamento == null)
            {
                throw new ArgumentNullException(nameof(relacionamento));
            }

            // Validação de parâmetros de entrada
            if (relacionamento.CoMiniappPai == Guid.Empty)
            {
                _logger.LogWarning("Código do mini app pai está vazio");
                throw new BusinessException("MINIAPP_PAI_INVALIDO", "O código do mini app pai não pode estar vazio.", 400);
            }

            if (relacionamento.CoMiniappFilho == Guid.Empty)
            {
                _logger.LogWarning("Código do mini app filho está vazio");
                throw new BusinessException("MINIAPP_FILHO_INVALIDO", "O código do mini app filho não pode estar vazio.", 400);
            }

            _logger.LogInformation("Iniciando criação de relacionamento entre Pai: {CoMiniappPai} e Filho: {CoMiniappFilho}", 
                relacionamento.CoMiniappPai, relacionamento.CoMiniappFilho);

            // Validação: um mini app não pode ser pai dele mesmo
            if (relacionamento.CoMiniappPai == relacionamento.CoMiniappFilho)
            {
                _logger.LogWarning("Tentativa de autorelacionamento rejeitada para mini app: {CoMiniapp}", relacionamento.CoMiniappPai);
                throw new BusinessException("MINIAPP_AUTORELACIONAMENTO", "Um mini app não pode ser pai dele mesmo.", 400);
            }

            var miniappPaiExiste = await _relacionamentoRepository.MiniappExisteAsync(relacionamento.CoMiniappPai);
            
            if (!miniappPaiExiste)
            {
                _logger.LogWarning("Mini app pai não encontrado: {CoMiniappPai}", relacionamento.CoMiniappPai);
                throw new BusinessException("MINIAPP_PAI_NAO_ENCONTRADO", $"Mini app pai com código {relacionamento.CoMiniappPai} não foi encontrado ou está inativo.", 404);
            }

            var miniappFilhoExiste = await _relacionamentoRepository.MiniappExisteAsync(relacionamento.CoMiniappFilho);
            _logger.LogDebug("Mini app filho existe: {Existe}", miniappFilhoExiste);
            
            if (!miniappFilhoExiste)
            {
                _logger.LogWarning("Mini app filho não encontrado: {CoMiniappFilho}", relacionamento.CoMiniappFilho);
                throw new BusinessException("MINIAPP_FILHO_NAO_ENCONTRADO", $"Mini app filho com código {relacionamento.CoMiniappFilho} não foi encontrado ou está inativo.", 404);
            }

            // Verificar se o relacionamento já existe
            _logger.LogDebug("Verificando se relacionamento já existe");
            var relacionamentoExiste = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(
                relacionamento.CoMiniappPai, relacionamento.CoMiniappFilho);
            _logger.LogDebug("Relacionamento já existe: {Existe}", relacionamentoExiste);
            
            if (relacionamentoExiste)
            {
                _logger.LogWarning("Relacionamento já existe entre Pai: {CoMiniappPai} e Filho: {CoMiniappFilho}", 
                    relacionamento.CoMiniappPai, relacionamento.CoMiniappFilho);
                throw new BusinessException("RELACIONAMENTO_JA_EXISTE", "Este relacionamento já existe entre os mini apps especificados.", 409);
            }

            _logger.LogDebug("Criando relacionamento no repositório");
            await _relacionamentoRepository.CriarRelacionamentoAsync(relacionamento);
            
            _logger.LogInformation("Relacionamento criado com sucesso entre mini app pai {CoMiniappPai} e filho {CoMiniappFilho}", 
                relacionamento.CoMiniappPai, relacionamento.CoMiniappFilho);
        }

        public async Task<ListaRelacionamentosDto> ListarRelacionamentosAsync(EntradaRelacionamentosDto entrada)
        {
            if (entrada == null)
            {
                throw new ArgumentNullException(nameof(entrada));
            }

            if (entrada.CoMiniapp == Guid.Empty)
            {
                _logger.LogWarning("Código do mini app está vazio para listagem de relacionamentos");
                throw new BusinessException("MINIAPP_INVALIDO", "O código do mini app não pode estar vazio.", 400);
            }

            if (string.IsNullOrWhiteSpace(entrada.Relacao))
            {
                _logger.LogWarning("Tipo de relação não especificado, usando padrão 'AMBOS'");
                entrada.Relacao = "AMBOS";
            }

            _logger.LogInformation("Listando relacionamentos para mini app: {CoMiniapp}, Relação: {Relacao}", 
                entrada.CoMiniapp, entrada.Relacao);

            // Verificar se o mini app existe
            var miniappExiste = await _relacionamentoRepository.MiniappExisteAsync(entrada.CoMiniapp);
            if (!miniappExiste)
            {
                _logger.LogWarning("Mini app não encontrado: {CoMiniapp}", entrada.CoMiniapp);
                throw new BusinessException("MINIAPP_NAO_ENCONTRADO", $"Mini app com código {entrada.CoMiniapp} não foi encontrado ou está inativo.", 404);
            }

            var resultado = new ListaRelacionamentosDto();

            switch (entrada.Relacao?.ToUpper())
            {
                case "PAIS":
                    resultado.pais = await _relacionamentoRepository.GetMiniappsPaisAsync(entrada.CoMiniapp);
                    break;
                case "FILHOS":
                    resultado.filhos = await _relacionamentoRepository.GetMiniappsFilhosAsync(entrada.CoMiniapp);
                    break;
                default:
                    resultado.pais = await _relacionamentoRepository.GetMiniappsPaisAsync(entrada.CoMiniapp);
                    resultado.filhos = await _relacionamentoRepository.GetMiniappsFilhosAsync(entrada.CoMiniapp);
                    break;
            }

            _logger.LogInformation("Relacionamentos listados - Pais: {QtdPais}, Filhos: {QtdFilhos}", 
                resultado.pais.Count, resultado.filhos.Count);

            return resultado;
        }

        public async Task ExcluirRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho)
        {
            _logger.LogInformation("Iniciando exclusão de relacionamento entre Pai: {CoMiniappPai} e Filho: {CoMiniappFilho}", 
                coMiniappPai, coMiniappFilho);

            // Validação de parâmetros de entrada
            if (coMiniappPai == Guid.Empty)
            {
                _logger.LogWarning("Código do mini app pai está vazio para exclusão");
                throw new BusinessException("MINIAPP_PAI_INVALIDO", "O código do mini app pai não pode estar vazio.", 400);
            }

            if (coMiniappFilho == Guid.Empty)
            {
                _logger.LogWarning("Código do mini app filho está vazio para exclusão");
                throw new BusinessException("MINIAPP_FILHO_INVALIDO", "O código do mini app filho não pode estar vazio.", 400);
            }

            // Verificar se o relacionamento existe antes de tentar excluir
            var relacionamentoExiste = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(coMiniappPai, coMiniappFilho);
            if (!relacionamentoExiste)
            {
                _logger.LogWarning("Relacionamento não encontrado para exclusão - Pai: {CoMiniappPai}, Filho: {CoMiniappFilho}", 
                    coMiniappPai, coMiniappFilho);
                throw new BusinessException("RELACIONAMENTO_NAO_ENCONTRADO", "Relacionamento não encontrado para os mini apps especificados.", 404);
            }

            // Realizar a exclusão
            await _relacionamentoRepository.ExcluirRelacionamentoAsync(coMiniappPai, coMiniappFilho);
            
            _logger.LogInformation("Relacionamento excluído com sucesso entre Pai: {CoMiniappPai} e Filho: {CoMiniappFilho}", 
                coMiniappPai, coMiniappFilho);
        }

        public async Task<List<RetornoRelacionamentosDto>> ListarTodosRelacionamentosAsync()
        {
            _logger.LogInformation("Listando todos os relacionamentos do sistema");
            
            var relacionamentos = await _relacionamentoRepository.GetTodosRelacionamentosAsync();
            
            _logger.LogInformation("Total de relacionamentos encontrados: {Total}", relacionamentos.Count);
            
            return relacionamentos;
        }

        public async Task<bool> ValidarRelacionamentoAsync(Guid coMiniappPai, Guid coMiniappFilho)
        {
            // Validação básica: não pode ser pai dele mesmo
            if (coMiniappPai == coMiniappFilho)
            {
                return false;
            }

            // Verificar se ambos os mini apps existem
            var miniappPaiExiste = await _relacionamentoRepository.MiniappExisteAsync(coMiniappPai);
            var miniappFilhoExiste = await _relacionamentoRepository.MiniappExisteAsync(coMiniappFilho);

            if (!miniappPaiExiste || !miniappFilhoExiste)
            {
                return false;
            }

            // Verificar se o relacionamento já existe
            var relacionamentoExiste = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(coMiniappPai, coMiniappFilho);
            
            // Retorna true se o relacionamento é válido (não existe ainda)
            return !relacionamentoExiste;
        }

        public async Task<bool> RelacionamentoExisteAsync(Guid coMiniappPai, Guid coMiniappFilho)
        {
            // Método específico para verificar apenas se o relacionamento existe
            // (diferente do ValidarRelacionamentoAsync que valida se é possível criar um novo)
            return await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(coMiniappPai, coMiniappFilho);
        }

        public async Task EditarRelacionamentoAsync(EditarRelacionamentoMiniappDto entrada)
        {
            if (entrada == null)
            {
                throw new ArgumentNullException(nameof(entrada));
            }

            _logger.LogInformation("Iniciando edição de relacionamento - Original: Pai: {CoMiniappPaiOriginal}, Filho: {CoMiniappFilhoOriginal} -> Novo: Pai: {CoMiniappPaiNovo}, Filho: {CoMiniappFilhoNovo}", 
                entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal, entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);

            // Validação usando propriedades do DTO
            if (!entrada.GUIDsValidos)
            {
                _logger.LogWarning("GUIDs inválidos na entrada de edição");
                throw new BusinessException("PARAMETROS_INVALIDOS", "Todos os códigos dos mini apps devem ser válidos.", 400);
            }

            if (!entrada.TemMudancas)
            {
                _logger.LogWarning("Tentativa de edição sem mudanças efetivas");
                throw new BusinessException("SEM_MUDANCAS", "Nenhuma alteração foi detectada nos dados do relacionamento.", 400);
            }

            // Validação: um mini app não pode ser pai dele mesmo
            if (entrada.CoMiniappPaiNovo == entrada.CoMiniappFilhoNovo)
            {
                _logger.LogWarning("Tentativa de autorelacionamento rejeitada para mini app: {CoMiniapp}", entrada.CoMiniappPaiNovo);
                throw new BusinessException("MINIAPP_AUTORELACIONAMENTO", "Um mini app não pode ser pai dele mesmo.", 400);
            }

            // Verificar se o relacionamento original existe
            var relacionamentoOriginalExiste = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal);
            if (!relacionamentoOriginalExiste)
            {
                _logger.LogWarning("Relacionamento original não encontrado - Pai: {CoMiniappPaiOriginal}, Filho: {CoMiniappFilhoOriginal}", entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal);
                throw new BusinessException("RELACIONAMENTO_NAO_ENCONTRADO", "Relacionamento original não encontrado para edição.", 404);
            }

            // Verificar se os novos mini apps existem
            var miniappPaiExiste = await _relacionamentoRepository.MiniappExisteAsync(entrada.CoMiniappPaiNovo);
            if (!miniappPaiExiste)
            {
                _logger.LogWarning("Novo mini app pai não encontrado: {CoMiniappPaiNovo}", entrada.CoMiniappPaiNovo);
                throw new BusinessException("MINIAPP_PAI_NAO_ENCONTRADO", $"Mini app pai com código {entrada.CoMiniappPaiNovo} não foi encontrado ou está inativo.", 404);
            }

            var miniappFilhoExiste = await _relacionamentoRepository.MiniappExisteAsync(entrada.CoMiniappFilhoNovo);
            if (!miniappFilhoExiste)
            {
                _logger.LogWarning("Novo mini app filho não encontrado: {CoMiniappFilhoNovo}", entrada.CoMiniappFilhoNovo);
                throw new BusinessException("MINIAPP_FILHO_NAO_ENCONTRADO", $"Mini app filho com código {entrada.CoMiniappFilhoNovo} não foi encontrado ou está inativo.", 404);
            }

            // Verificar se o novo relacionamento já existe (apenas se houver mudanças)
            var novoRelacionamentoExiste = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(
                entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
            
            if (novoRelacionamentoExiste)
            {
                _logger.LogWarning("Novo relacionamento já existe entre Pai: {CoMiniappPaiNovo} e Filho: {CoMiniappFilhoNovo}", 
                    entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
                throw new BusinessException("RELACIONAMENTO_JA_EXISTE", "O novo relacionamento já existe entre os mini apps especificados.", 409);
            }

            // Operação transacional mais robusta
            try
            {
                _logger.LogDebug("Iniciando operação transacional de edição");
                
                // Primeiro, criar o novo relacionamento
                _logger.LogDebug("Criando novo relacionamento");
                var novoRelacionamento = new IncluirRelacionamentoMiniappDto
                {
                    CoMiniappPai = entrada.CoMiniappPaiNovo,
                    CoMiniappFilho = entrada.CoMiniappFilhoNovo
                };
                await _relacionamentoRepository.CriarRelacionamentoAsync(novoRelacionamento);
                
                // Depois, excluir o relacionamento original
                _logger.LogDebug("Excluindo relacionamento original");
                await _relacionamentoRepository.ExcluirRelacionamentoAsync(entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal);
                
                _logger.LogInformation("Relacionamento editado com sucesso - Antigo: Pai: {CoMiniappPaiAntigo}, Filho: {CoMiniappFilhoAntigo} -> Novo: Pai: {CoMiniappPaiNovo}, Filho: {CoMiniappFilhoNovo}", 
                    entrada.CoMiniappPaiOriginal, entrada.CoMiniappFilhoOriginal, entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante operação transacional de edição - tentando reverter");
                
                // Tentar reverter: se o novo relacionamento foi criado, tentar excluí-lo
                try
                {
                    var novoRelacionamentoCriado = await _relacionamentoRepository.VerificarRelacionamentoExisteAsync(
                        entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
                    
                    if (novoRelacionamentoCriado)
                    {
                        _logger.LogDebug("Tentando reverter: excluindo novo relacionamento criado");
                        await _relacionamentoRepository.ExcluirRelacionamentoAsync(entrada.CoMiniappPaiNovo, entrada.CoMiniappFilhoNovo);
                    }
                }
                catch (Exception reverterEx)
                {
                    _logger.LogError(reverterEx, "Falha ao reverter operação - estado inconsistente possível");
                }
                
                throw new BusinessException("ERRO_EDICAO_RELACIONAMENTO", "Erro durante a edição do relacionamento. Operação foi revertida.", 500);
            }
        }
    }
}