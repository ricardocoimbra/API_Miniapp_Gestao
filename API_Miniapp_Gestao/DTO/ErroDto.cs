using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace API_Miniapp_Gestao.DTO;

[ExcludeFromCodeCoverage]
public class ErroDto
{
    public ErroDto() { }

    public ErroDto(string codigo, string mensagem, int statusCode)
    {
        Codigo = codigo;
        Mensagem = mensagem;
        StatusCode = statusCode;
    }

    [JsonPropertyName("codigo")]
    [SwaggerSchema(Description = "Código do erro.")]
    public string? Codigo { get; set; } = string.Empty;
    [JsonPropertyName("mensagem")]
    [SwaggerSchema(Description = "Mensagem do erro.")]
    public string? Mensagem { get; set; } = string.Empty;
    [JsonPropertyName("reenviar")]
    [SwaggerSchema(Description = "Se o erro permitir o reenvio.")]
    public bool? Reenviar { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }
}
