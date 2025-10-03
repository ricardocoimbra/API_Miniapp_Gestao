using System.Diagnostics.CodeAnalysis;
using API_Miniapp_Gestao.DTO;

namespace API_Miniapp_Gestao.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class HttpCustomException : Exception
    {
        public ErroDto Erro { get; }

        public HttpCustomException(ErroDto erro)
        {
            Erro = new ErroDto
            {
                Codigo = erro.Codigo,
                Mensagem = erro.Mensagem
            };
        }
    }
}
