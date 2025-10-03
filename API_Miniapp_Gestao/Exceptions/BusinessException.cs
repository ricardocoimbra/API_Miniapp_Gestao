using System.Diagnostics.CodeAnalysis;
using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BusinessException : Exception
    {
        public readonly ErroDto Erro;
        public BusinessException(ErroDto? erro)
        {
            Erro = erro!;
        }
        public BusinessException(string? codigo, string? mensagem, int statusCode)
        {
            Erro = new ErroDto
            {
                Codigo = codigo,
                Mensagem = mensagem,
                StatusCode = statusCode
            };
        }

        public static BusinessException Throw(string? codigo, string? mensagem, int statusCode)
        {
            return new BusinessException(new ErroDto
            {
                Codigo = codigo,
                Mensagem = mensagem,
                StatusCode = statusCode
            });
        }
    }
}
