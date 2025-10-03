using System.Net;
using API_Miniapp_Gestao.Audit;
using API_Miniapp_Gestao.Config;
using API_Miniapp_Gestao.Enums;
using API_Miniapp_Gestao.Exceptions;
using API_Miniapp_Gestao.Models;
using API_Miniapp_Gestao.Repositories;
using API_Miniapp_Gestao.Repositories.Interface;
using API_Miniapp_Gestao.Services;
using API_Miniapp_Gestao.Services.Interfaces;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Adiciona serviços ao contêiner.
services.AddHealthChecks();
services.AddControllers();

// Adiciona as configurações personalizadas
services.Configure<TimeoutConfig>(configuration.GetSection("TimeoutConfig"));
services.Configure<ServicesConfig>(configuration.GetSection("ServicesConfig"));
services.Configure<CacheConfig>(configuration.GetSection("CacheConfig"));

// Configuração do cliente do Key Vault com proxy em desenvolvimento
SecretClient keyVaultClient;
if (builder.Environment.IsDevelopment())
{
    var isDevelopment = true;
    keyVaultClient = getSecretClient(isDevelopment);
}
else
{
    keyVaultClient = getSecretClient();
}

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
});

// Busca a API Key no Key Vault
var apiSecret = await keyVaultClient.GetSecretAsync("ServicesConfigApiKey");
string? apiKey = apiSecret.Value.Value;


services.AddApplicationInsightsTelemetry();
services.AddSingleton(keyVaultClient);
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API-Miniapp-Gestão",
        Version = "V1",
    });
});

services.AddHeaderPropagation(options =>
{
    options.Headers.Add("Authorization");
});

services.AddHttpClient();
services.AddHttpContextAccessor();

// Busca a connection string de auditoria no Key Vault
string? auditoriaConnectionString = GetConnectionStringFromKeyVault("Auditoria:EhTrilhaAuditoriaSender", keyVaultClient);

// Configura o serviço de auditoria com dados do Key Vault e appsettings
builder.Services.AddAuditoriaService(
    auditoriaConnectionString,
    builder.Configuration.GetValue<string>("Auditoria:EventHubName"),
    builder.Configuration.GetValue<string>("Auditoria:NomeAplicacao")
);

// Configura o HttpClient para chamadas
services.AddHttpClient(NomeHttpClient.ApisComAuditoria.ToString(), httpClient =>
{
    httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ClientCertificateOptions = ClientCertificateOption.Manual,
    // Aceita qualquer certificado de servidor (não recomendado para produção)
    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
{
    return true;
}
})
.AddHttpMessageHandler<ApisComAuditoriaHandler>() // Handler customizado para auditoria
.AddHeaderPropagation(); // Propaga headers

// Injeção de dependências para serviços e repositórios
services.AddScoped<IMiniAppService, MiniAppService>();
services.AddScoped<IMiniAppRepository, MiniAppRepository>();
services.AddScoped<IMiniappVersaoService, MiniappVersaoService>();
services.AddScoped<IMiniAppVersaoRepository, MiniAppVersaoRepository>();
services.AddScoped<IRelacionamentoMiniappService, RelacionamentoMiniappService>();
services.AddScoped<IRelacionamentoMiniappRepository, RelacionamentoMiniappRepository>();

// Configura o Entity Framework com SQL Server e connection strings do Key Vault
string? connectionStringLeitura = GetConnectionStringFromKeyVault("ConnectionStrings:ConnectionStringDbSuperAppLeitura", keyVaultClient);
services.AddDbContext<DbLeitura>(
    options =>
    {
        options.UseSqlServer(connectionStringLeitura,
            providerOptions => providerOptions.EnableRetryOnFailure()).EnableSensitiveDataLogging();
    });
string? connectionStringEscrita = GetConnectionStringFromKeyVault("ConnectionStrings:ConnectionStringDbSuperAppEscrita", keyVaultClient);
services.AddDbContext<DbEscrita>(
    options =>
    {
        options.UseSqlServer(connectionStringEscrita,
            providerOptions => providerOptions.EnableRetryOnFailure()).EnableSensitiveDataLogging();
    });

#region APP Build

var app = builder.Build();

// habilitar cors adicionado
app.UseCors("AllowAny");
app.MapHealthChecks("/healthz"); // Mapeia endpoint de health check
app.UseMiddleware<SegurancaMiddleware>(); // Middleware de segurança requisições
app.UseMiddleware<AuditoriaMiddleware>(); // Middleware de auditoria se configurado
app.UseMetricServer(); // Middleware para métricas Prometheus
app.UseCustomExceptionHandler(); // Middleware para tratamento customizado de exceções

app.UseSwagger(c => c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API-Miniapp-Gestao v1");
});

app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS
app.UseHeaderPropagation();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

#endregion APP Build



//Métodos auxiliares
string GetConnectionStringFromKeyVault(string appSettingsSecretName, SecretClient keyVaultClient)
{
    var appsettingsSecret = configuration.GetValue<string>(appSettingsSecretName);
    var secret = keyVaultClient.GetSecret(appsettingsSecret);
    return secret.Value.Value;
}

SecretClient getSecretClient(bool isDevelopment = false)
{
    var urlKeyVault = configuration.GetValue<string>("UrlKeyVault");
    if (isDevelopment)
    {
        var urlWebProxy = $"http://prd-internet365.caixa:80";
        var httpClientHandler = new HttpClientHandler()
        {
            Proxy = new WebProxy
            {
                Address = new Uri(urlWebProxy),
                UseDefaultCredentials = true, // Usa credenciais do Windows/AD
                BypassProxyOnLocal = true
            },
            UseProxy = true
        };

        var httpClient = new HttpClient(httpClientHandler);
        var clientOptions = new SecretClientOptions()
        {
            Transport = new HttpClientTransport(httpClient)
        };
        return new SecretClient(new Uri(urlKeyVault!), new DefaultAzureCredential(), clientOptions);
    }
    return new SecretClient(new Uri(urlKeyVault!), new DefaultAzureCredential());
}

void AddBasicCors()
{
    // CORS
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAny", builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
    });
}

[ExcludeFromCodeCoverage]
public partial class Program
{
    protected Program() { }
}