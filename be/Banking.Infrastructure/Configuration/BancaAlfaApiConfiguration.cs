namespace Banking.Infrastructure.Configuration;

public class BancaAlfaApiConfiguration
{
    public const string SectionName = "BancaAlfaApi";
    
    public string BaseUrl { get; set; } = "https://api.bancaalfa.it";
    public int TimeoutSeconds { get; set; } = 30;
    public string ApiVersion { get; set; } = "v1";
    public bool EnableRetryPolicy { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
}
