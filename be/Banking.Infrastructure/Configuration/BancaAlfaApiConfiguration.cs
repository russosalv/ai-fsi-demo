namespace Banking.Infrastructure.Configuration;

public class BancaAlfaApiConfiguration
{
    public string BaseUrl { get; set; } = "https://api.bancaalfa.it";
    public string P2PTransferEndpoint { get; set; } = "/v1/payments/p2p-transfer";
    public int TimeoutSeconds { get; set; } = 30;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
