using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra i servizi dell'infrastruttura per Banca Alfa
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection per chaining</returns>
    public static IServiceCollection AddBancaAlfaInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {        // Configurazione
        services.Configure<BancaAlfaApiConfiguration>(
            configuration.GetSection("BancaAlfaApi"));        // HTTP Client per Banca Alfa con nome specifico
        services.AddHttpClient<IBancaAlfaP2PService, BancaAlfaP2PService>("BancaAlfa")
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var config = configuration.GetSection("BancaAlfaApi").Get<BancaAlfaApiConfiguration>();
                if (config != null)
                {
                    client.BaseAddress = new Uri(config.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                }
            });

        // Registrazione servizi
        services.AddScoped<IBancaAlfaP2PService, BancaAlfaP2PService>();

        return services;
    }
}
