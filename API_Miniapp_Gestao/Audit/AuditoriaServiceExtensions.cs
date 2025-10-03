using System.Diagnostics.CodeAnalysis;
using API_Miniapp_Gestao.Helpers;

namespace API_Miniapp_Gestao.Audit;

[ExcludeFromCodeCoverage]
public static class AuditoriaServiceExtensions
{
    private static readonly string mensagem = "Não foi possível iniciar o serviço de auditoria. Motivo: O parâmetro '{0}' não é válido. Confira as configurações do 'appsettings.json'.";

    public static IServiceCollection AddAuditoriaService(this IServiceCollection services, string? connectionString, string? eventHubName, string? nomeAplicacao)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new AuditoriaException(string.Format(mensagem, "ConnectionString"));

        if (string.IsNullOrWhiteSpace(eventHubName))
            throw new AuditoriaException(string.Format(mensagem, "EventHubName"));

        if (string.IsNullOrWhiteSpace(nomeAplicacao))
            throw new AuditoriaException(string.Format(mensagem, "NomeAplicacao"));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new("ConnectionString", connectionString),
            new("EventHubName", eventHubName),
            new("NomeAplicacao", nomeAplicacao),
        }).Build();
        services.Configure<AuditoriaConfig>(config);

        services.AddSingleton<AuditoriaService, AuditoriaService>();
        services.AddSingleton<MascaraDadosSensiveis, MascaraDadosSensiveis>();
        services.AddScoped<ApisComAuditoriaHandler>();
        services.AddScoped<ApisSemAuditoriaHandler>();
        services.AddScoped<AuditoriaCacheService, AuditoriaCacheService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
