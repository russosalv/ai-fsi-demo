using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Banking.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBancaAlfaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurazione
        services.Configure<BancaAlfaApiConfiguration>(
            configuration.GetSection(BancaAlfaApiConfiguration.SectionName));

        // HttpClient con Polly retry policy
        services.AddHttpClient<IBancaAlfaApiClient, BancaAlfaApiClient>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<BancaAlfaApiConfiguration>>().Value;
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        })
        .AddPolicyHandler((serviceProvider, request) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<BancaAlfaApiConfiguration>>().Value;
            
            if (!config.EnableRetryPolicy)
                return Policy.NoOpAsync<HttpResponseMessage>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: config.MaxRetryAttempts,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(
                        config.RetryDelayMilliseconds * Math.Pow(2, retryAttempt - 1)), // Exponential backoff
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<BancaAlfaApiClient>>();
                        logger?.LogWarning("Retry attempt {RetryCount} for {RequestUri} after {Delay}ms", 
                            retryCount, request.RequestUri, timespan.TotalMilliseconds);
                    });
        });

        // Servizi
        services.AddScoped<IP2PTransferService, P2PTransferService>();

        return services;
    }
}
