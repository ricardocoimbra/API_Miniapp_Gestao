using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.Audit;

[ExcludeFromCodeCoverage]
public class AuditoriaDto
{
    public string? Id { get; set; }
    public string? CoId { get; set; }
    [JsonPropertyName("aplicacao")] public string? NomeDaAplicacao { get; set; }
    [JsonPropertyName("req-acao")] public string? ReqAcao { get; set; }
    [JsonPropertyName("req-baseurl")] public string? ReqBaseUrl { get; set; }
    [JsonPropertyName("req-body")] public string? ReqBody { get; set; }
    [JsonPropertyName("req-headers")] public Dictionary<string, string[]>? ReqHeaders { get; set; }
    [JsonPropertyName("req-method")] public string? ReqMethod { get; set; }
    [JsonPropertyName("req-time")] public DateTime ReqTime { get; set; }
    [JsonPropertyName("req-token-payload")] public Dictionary<string, object>? ReqTokenPayload { get; set; }
    [JsonPropertyName("req-url")] public string? ReqUrl { get; set; }
    [JsonPropertyName("rsp-body")] public string? RspBody { get; set; }
    [JsonPropertyName("rsp-headers")] public Dictionary<string, string[]>? RspHeaders { get; set; }
    [JsonPropertyName("rsp-status")] public string? RspStatus { get; set; }
    [JsonPropertyName("user-name")] public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("tempo-resposta")] public long? TempoResposta { get; set; }

    public string? DadosNegociais { get; set; }

    public AuditoriaDto(
        string requestBody,
        string responseBody,
        HttpContext context,
        string acao,
        string nomeDaAplicacao,
        string coId = "",
        string dadosNegociais = "",
        long? tempoResposta = null)
    {
        Id = Guid.NewGuid().ToString();
        CoId = coId;
        NomeDaAplicacao = nomeDaAplicacao;
        ReqAcao = acao;
        DadosNegociais = dadosNegociais;
        ReqBaseUrl = context.Request.GetEncodedPathAndQuery() ?? string.Empty;
        ReqBody = requestBody;
        ReqMethod = context.Request.Method;
        ReqTime = DateTime.Now;
        ReqUrl = context.Request.Host.Value;
        ReqHeaders = GetHeaders([.. context.Request.Headers], context.Response.StatusCode);
        RspBody = responseBody;
        RspHeaders = [];
        context.Response.Headers.ToList().ForEach(h =>
        {
            RspHeaders.Add(h.Key, [h.Value.ToString()]);
        });
        RspStatus = context.Response.StatusCode.ToString();
        TempoResposta = tempoResposta;
    }

    public AuditoriaDto(
        HttpRequestMessage request,
        string requestBody,
        HttpResponseMessage response,
        string responseBody,
        string acao,
        string nomeDaAplicacao,
        string coId = "",
        string dadosNegociais = "",
        long? tempoResposta = null)
    {
        Id = Guid.NewGuid().ToString();
        CoId = coId;
        NomeDaAplicacao = nomeDaAplicacao;
        ReqAcao = acao;
        DadosNegociais = dadosNegociais;
        ReqBaseUrl = request?.RequestUri?.PathAndQuery ?? string.Empty;
        ReqBody = requestBody;
        ReqMethod = request?.Method.Method ?? string.Empty;
        ReqTime = DateTime.Now;
        ReqUrl = request?.RequestUri?.Host ?? string.Empty;
        ReqHeaders = GetHeaders(request?.Headers?.ToList() ?? [], response.StatusCode);
        RspBody = responseBody;
        RspHeaders = GetHeaders(response.Headers.ToList(), response.StatusCode);
        RspStatus = ((int)response.StatusCode).ToString();
        TempoResposta = tempoResposta;
    }

    public AuditoriaDto(
        string baseUrl,
        string method,
        string path,
        Dictionary<string, string[]> requestheaders,
        string requestBody,
        string respStatusCode,
        string responseBody,
        Dictionary<string, string[]> respHeaders,
        string acao,
        string nomeDaAplicacao,
        string coId = "",
        string dadosNegociais = "",
        long? tempoResposta = null)
    {
        Id = Guid.NewGuid().ToString();
        CoId = coId;
        NomeDaAplicacao = nomeDaAplicacao;
        ReqAcao = acao;
        DadosNegociais = dadosNegociais;
        ReqBaseUrl = baseUrl;
        ReqBody = requestBody;
        ReqMethod = method;
        ReqTime = DateTime.Now;
        ReqUrl = path;
        ReqHeaders = requestheaders;
        RspBody = responseBody;
        RspHeaders = respHeaders;
        RspStatus = respStatusCode;
        TempoResposta = tempoResposta;
    }

    private Dictionary<string, string[]> GetHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, HttpStatusCode statusCode)
    {
        var headersDic = new Dictionary<string, string[]>();

        headers.ToList().ForEach(h =>
        {
            if ("Authorization".Equals(h.Key) && statusCode != HttpStatusCode.Unauthorized)
            {
                JwtSecurityTokenHandler tokenHandler = new();
                string token = h.Value.GetType().Equals(typeof(string[]))
                    ? h.Value.FirstOrDefault(x => x.Contains("Bearer"))?.Replace("Bearer", string.Empty).Trim() ?? ""
                    : h.Value.ToString()?.Replace("Bearer", string.Empty).Trim() ?? "";

                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                ReqTokenPayload = jwtToken?.Payload;

                UserName = string.Empty;
                if (ReqTokenPayload != null && ReqTokenPayload.ContainsKey("preferred_username"))
                    UserName = ReqTokenPayload["preferred_username"].ToString()!;
            }
            else
            {
                headersDic.Add(h.Key, h.Value.ToArray());
            }
        });

        return headersDic;
    }

    private Dictionary<string, string[]> GetHeaders(List<KeyValuePair<string, StringValues>> headers, int statusCode)
    {
        var headersDic = new Dictionary<string, string[]>();

        headers.ToList().ForEach(h =>
        {
            if ("Authorization".Equals(h.Key) && statusCode != (int)HttpStatusCode.Unauthorized)
            {
                JwtSecurityTokenHandler tokenHandler = new();

                string token = h.Value.GetType().Equals(typeof(string[]))
                    ? h.Value.FirstOrDefault(x => x.Contains("Bearer"))?.Replace("Bearer", string.Empty).Trim() ?? ""
                    : h.Value.ToString().Replace("Bearer", string.Empty).Trim();

                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                ReqTokenPayload = jwtToken?.Payload;

                UserName = string.Empty;
                if (ReqTokenPayload != null && ReqTokenPayload.ContainsKey("preferred_username"))
                    UserName = ReqTokenPayload["preferred_username"].ToString()!;
            }
            else
            {
                headersDic.Add(h.Key, [h.Value.ToString()]);
            }
        });

        return headersDic;
    }
}
