using System.Text;
using API_Miniapp_Gestao.DTO;
using System.Text.Json;
using System.Text.Json.Serialization;
using API_Miniapp_Gestao.Config;
using Microsoft.Extensions.Options;
using API_Miniapp_Gestao.Security;
using API_Miniapp_Gestao.Helpers;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Middleware responsável por validar regras de segurança e predecessão de etapas via Redis.
/// Realiza validação de etapas anteriores, insere etapa atual no cache e retorna erro de acesso quando necessário.
/// </summary>
[ExcludeFromCodeCoverage]
public class SegurancaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SegurancaMiddleware> _logger;
    private readonly HttpClient _httpClient;
    private readonly RedisConnectionHelper? _redisConnectionHelper;
    private readonly CacheConfig _cacheConfig;

    // Prefixo para chave de cache no Redis
    private readonly string _chaveCache = "PREDECESSAO_";

    /// <summary>
    /// Construtor do middleware, recebe dependências via injeção.
    /// RedisConnectionHelper é opcional - se não estiver disponível, as funcionalidades de cache são desabilitadas.
    /// </summary>
    public SegurancaMiddleware(RequestDelegate proximo, ILogger<SegurancaMiddleware> logger, HttpClient httpClient, IOptions<CacheConfig> cacheConfig, RedisConnectionHelper? redisConnectionHelper = null)
    {
        _next = proximo;
        _logger = logger;
        _httpClient = httpClient;
        _redisConnectionHelper = redisConnectionHelper;
        _cacheConfig = cacheConfig.Value;

        if (_redisConnectionHelper == null)
        {
            _logger.LogInformation("SegurancaMiddleware: Redis não disponível. Funcionalidades de cache desabilitadas.");
        }
    }

    /// <summary>
    /// M�todo principal do middleware, executado a cada requisi��o.
    /// Valida regras de predecess�o antes e insere etapa atual ap�s o processamento.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Chamar API de seguran�a para decriptografar, validar e recuperar token

        // Recupera atributo de predecess�o do endpoint
        var atributte = context.GetEndpoint()?.Metadata.GetMetadata<PredecessaoAttribute>();
        // Se o m�todo est� anotado com [Predecessao], valida regras de predecess�o
        if (atributte != null)
        {
            await ValidaPredecessao(context, atributte);
        }
        var atributteAssinatura = context.GetEndpoint()?.Metadata.GetMetadata<AssinaturaEletronicaAttribute>();
        // Se o m�todo est� anotado com [Predecessao], valida regras de predecess�o
        if (atributteAssinatura != null)
        {
            //TODO, validar se o body contem assinatura eletronica preenchida e chamar API para valida��o
        }

        // Chama o pr�ximo middleware da pipeline
        await _next(context);

        // Ap�s processamento, insere etapa atual no Redis se houver atributo de predecess�o
        if (atributte != null)
        {
            await SetaPredecessao(context, atributte);
        }
        // TODO: Chamar API de seguran�a para criptografar e assinar
    }

    /// <summary>
    /// Insere a etapa atual no Redis, associando ao SessionId.
    /// </summary>
    private async Task SetaPredecessao(HttpContext context, PredecessaoAttribute atributte)
    {
        if (_redisConnectionHelper == null)
        {
            _logger.LogWarning("Redis não disponível. Não foi possível definir predecessão para a sessão.");
            return;
        }

        // Recupera SessionId do header
        var sessionId = context.Request.Headers["SessionId"].ToString();
        // Adiciona etapa atual no Redis com tempo de expiração configurado
        await _redisConnectionHelper.setRegister(_chaveCache + sessionId, atributte.AposChamada.ToString(), _cacheConfig.TTLPredecessao);
        _logger.LogInformation("SessionId {SessionId} adicionado ao Redis.", sessionId);
    }

    /// <summary>
    /// Valida se o SessionId possui as etapas predecessoras necess�rias no Redis.
    /// Retorna erro de acesso caso n�o esteja conforme as regras.
    /// </summary>
    private async Task ValidaPredecessao(HttpContext context, PredecessaoAttribute atributte)
    {
        // Recupera SessionId do header
        var sessionId = context.Request.Headers["SessionId"].ToString();

        // Se SessionId n�o informado, retorna erro de acesso
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            await ErroAcesso(context);
        }
        // Se há regras de predecessão, valida etapa atual no Redis
        if (atributte.Anteriores.Count() > 0)
        {
            if (_redisConnectionHelper == null)
            {
                _logger.LogWarning("Redis não disponível. Não foi possível validar predecessão para a sessão {SessionId}.", sessionId);
                await ErroAcesso(context);
                return;
            }

            // Recupera etapa atual do cache
            var statusAtual = await _redisConnectionHelper.getRegister(_chaveCache + sessionId);
            if (statusAtual == null || statusAtual == "")
            {
                _logger.LogWarning("SessionId {SessionId} não encontrado no Redis.", sessionId);
                await ErroAcesso(context);
                return;
            }
            // Converte etapa atual para inteiro
            var etapaAtual = int.Parse(statusAtual);
            // Verifica se etapa atual está na lista de predecessoras permitidas
            if (!atributte.Anteriores.Contains(etapaAtual))
            {
                _logger.LogWarning("SessionId {SessionId} não é permitido para esta etapa.", sessionId);
                await ErroAcesso(context);
            }
        }
    }

    /// <summary>
    /// Retorna erro de acesso 403 em formato JSON.
    /// </summary>
    private async Task ErroAcesso(HttpContext context)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        _logger.LogWarning("SessionId n�o informado no header da requisi��o.");
        var erro = new List<ErroDto>
                            {
                                new ErroDto
                                {
                                    StatusCode= 403,
                                    Codigo ="E_SEG_PRE_403",
                                    Mensagem = "Erro de Acesso"
                                }

                };
        context.Response.StatusCode = erro.First().StatusCode;
        context.Response.ContentType = "application/json";
        // Retorna os erros no formato JSON
        await context.Response.WriteAsync(
            Encoding.UTF8.GetString(
                JsonSerializer.SerializeToUtf8Bytes(
                    erro, jsonSerializerOptions)));
    }
}
