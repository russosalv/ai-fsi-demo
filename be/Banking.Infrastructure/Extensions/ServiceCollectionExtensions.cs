using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{    /// <summary>
    /// Registra i servizi dell'infrastruttura per Banca Alfa
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="enableMocking">Se true, abilita il mocking del servizio invece della implementazione reale</param>
    /// <returns>Service collection per chaining</returns>
    public static IServiceCollection AddBancaAlfaInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        bool enableMocking = false)    {        // Configurazione
        services.Configure<BancaAlfaApiConfiguration>(
            configuration.GetSection("BancaAlfaApi"));

        // Determina se utilizzare il mock in base al parametro o alla configurazione
        var config = configuration.GetSection("BancaAlfaApi").Get<BancaAlfaApiConfiguration>();
        var shouldUseMock = enableMocking || (config?.EnableMocking ?? false);

        if (shouldUseMock)
        {
            // Registra il servizio mock
            services.AddScoped<IBancaAlfaP2PService, BancaAlfaP2PMockService>();
        }
        else
        {
            // HTTP Client per Banca Alfa con nome specifico
            services.AddHttpClient<IBancaAlfaP2PService, BancaAlfaP2PService>("BancaAlfa")
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    if (config != null)
                    {
                        client.BaseAddress = new Uri(config.BaseUrl);
                        client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                    }
                });

            // Registrazione servizio reale
            services.AddScoped<IBancaAlfaP2PService, BancaAlfaP2PService>();
        }

        return services;
    }
}
