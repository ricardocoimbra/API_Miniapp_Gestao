using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace API_Miniapp_Gestao.DTO
{
    [ExcludeFromCodeCoverage]
    public class EntradaPadraoSuperAppDto
    {
        //conta de origem
        [JsonPropertyName("contaOrigem")]
        public ContaDto? ContaOrigem { get; set; } = null;

        //cpf do cliente
        [JsonPropertyName("cpfCliente")]
        public decimal? CpfCliente { get; set; } = null;

        //contrato de Credito
        [JsonPropertyName("contratoCredito")]
        public ContratoCreditoDto? ContratoCredito { get; set; } = null;

        //cartao
        [JsonPropertyName("cartao")]
        public CartaoDto? Cartao { get; set; } = null;

        [JsonPropertyName("assinatura")]
        public AssinaturaDto? Assinatura { get; set; } = null;

        [JsonPropertyName("dadosDispositivo")]
        public DadosDispositivoDto? DadosDispositivo { get; set; } = null;

        [JsonPropertyName("dadosLocalizacao")]
        public DadosLocalizacaoDto? DadosLocalizacao { get; set; } = null;

        //Assinatura eletronica
        public class AssinaturaDto
        {
            [JsonPropertyName("assinatura")]
            public required string Assinatura { get; set; }
        }

        //Cartao 
        public class CartaoDto
        {
            [JsonPropertyName("numeroCartao")]
            public decimal NumeroCartao { get; set; }
            [JsonPropertyName("modalidade")]
            public string? Modalidade { get; set; } = null;
            [JsonPropertyName("bandeira")]
            public string? Bandeira { get; set; } = null;
            [JsonPropertyName("produto")]
            public short? Produto { get; set; } = null;
        }

        //contrato de credito
        public class ContratoCreditoDto
        {
            [JsonPropertyName("numeroContrato")]
            public decimal NumeroContrato { get; set; }
            [JsonPropertyName("codigoSistema")]
            public short? CodigoSistema { get; set; } = null;
            [JsonPropertyName("nomeSistema")]
            public string? NomeSistema { get; set; } = null;
            [JsonPropertyName("produto")]
            public short? Produto { get; set; } = null;
        }

        //Dto de conta
        public class ContaDto
        {
            [JsonPropertyName("unidade")]
            public short Unidade { get; set; }
            [JsonPropertyName("operacao")]
            public short Operacao { get; set; }
            [JsonPropertyName("conta")]
            public int Conta { get; set; }
            [JsonPropertyName("dv")]
            public char DV { get; set; }
        }

        public class DadosDispositivoDto
        {
            [JsonPropertyName("versaoAplicativo")]
            public string? VersaoAplicativo { get; set; } = null;
            [JsonPropertyName("porta")]
            public string? Porta { get; set; } = null;
            [JsonPropertyName("ipDispositivo")]
            public string? IpDispositivo { get; set; } = null;
            [JsonPropertyName("idDispositivo")]
            public string? IdDispositivo { get; set; } = null;
            [JsonPropertyName("sistemaOperacional")]
            public string? SistemaOperacional { get; set; } = null;
            [JsonPropertyName("versaoSistemaOperacional")]
            public string? VersaoSistemaOperacional { get; set; } = null;
        }

        public class DadosLocalizacaoDto
        {
            [JsonPropertyName("latitude")]
            public string? Latitude { get; set; } = null;
            [JsonPropertyName("longitude")]
            public string? Longitude { get; set; } = null;
            [JsonPropertyName("precisao")]
            public string? Precisao { get; set; } = null;
            [JsonPropertyName("timestamp")]
            public string? Timestamp { get; set; } = null;
        }

    }
}
