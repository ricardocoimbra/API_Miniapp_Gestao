using API_Miniapp_Gestao.Helpers;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace API_Miniapp_Gestao.Audit;

/// <summary>
/// Serviço responsável pelo envio dos dados de auditoria para o Azure Event Hubs.
/// </summary>
[ExcludeFromCodeCoverage]
public class AuditoriaService
{
    private readonly ILogger<AuditoriaService> _logger;
    private readonly string _nomeDaAplicacao;
    private readonly EventHubProducerClient _producerClient;
    private readonly TelemetryClient _telemetryClient;
    private readonly MascaraDadosSensiveis? _mascaraDadosSensiveis;

    public AuditoriaService(
        IOptions<AuditoriaConfig> configuration,
        ILogger<AuditoriaService> logger,
        TelemetryClient telemetryClient,
        MascaraDadosSensiveis? mascaraDadosSensiveis)
    {
        _logger = logger;

        var config = configuration.Value;
        _nomeDaAplicacao = config.NomeAplicacao;
        _producerClient = new EventHubProducerClient(config.ConnectionString, config.EventHubName);

        _telemetryClient = telemetryClient;
        _mascaraDadosSensiveis = mascaraDadosSensiveis;
    }

    /// <summary>
    /// Faz o envio dos dados de auditoria para o EH de forma paralela (o fluxo de aplicação seguirá,
    /// independentemente do resultado desta chamada).<br/>
    /// Para tanto, suas chamadas deverão ser feitas discartando seu retorno.<br/>
    /// Exemplo de utilização: _ = _auditoriaService.SendAsync(auditoriaDTO);
    /// </summary>
    public async Task SendAsync(AuditoriaDto auditoriaDTO)
    {
        try
        {
            var timer = new Stopwatch();
            timer.Start();
            _logger.LogDebug("<<Auditoria>> Antes de enviar mensagem para EH.");

            if (string.IsNullOrEmpty(auditoriaDTO.NomeDaAplicacao))
                auditoriaDTO.NomeDaAplicacao = _nomeDaAplicacao;

            auditoriaDTO = _mascaraDadosSensiveis?.MascararDados(auditoriaDTO) ?? auditoriaDTO;

            await handleMessageAsync(auditoriaDTO);

            timer.Stop();
            _logger.LogDebug("<<Auditoria>> Depois de enviar mensagem para EH. Tempo gasto em milisegundos: {Tempo}ms", timer.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "<<Auditoria>> Exception capturada no envio para o EH.");
            _telemetryClient.TrackTrace("ERRO_ENVIO_TRILHA_AUDITORIA", new Dictionary<string, string>()
            {
                { "erro", ex.Message }
            });
        }
    }

    private async Task handleMessageAsync(AuditoriaDto dto)
    {
        var mensagemEnviada = false;
        int count = 0;

        while (!mensagemEnviada)
        {
            using EventDataBatch edb = await _producerClient.CreateBatchAsync();
            if (edb.TryAdd(new EventData(JsonSerializer.SerializeToUtf8Bytes(dto))))
            {
                await _producerClient.SendAsync(edb);
                mensagemEnviada = true;
            }
            else
            {
                // Primeiro teste de tamanho do payload, retirar o response body
                if (count == 0)
                {
                    // Excluindo a resposta no caso de uma msg muito grande
                    dto.RspBody = "<<Payload muito grande, excluído para enviar para auditoria.>>";
                }
                else if (count == 1)
                {
                    dto.ReqBody = "<<Payload muito grande, excluído para enviar para auditoria.>>";
                }
                else if (count == 2)
                {
                    // Caso de contingência, não deverá acontecer, no entanto, se ocorrer, acaba o loop e envia o log de erro.
                    _logger.LogError("<<Auditoria>> Falha ao enviar mensagem. Motivo: Evento muito grande.");
                    mensagemEnviada = true;
                }

            }

            count++;
        }
    }
}
