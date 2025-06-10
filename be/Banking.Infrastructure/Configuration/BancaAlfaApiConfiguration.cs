namespace Banking.Infrastructure.Configuration;

public class BancaAlfaApiConfiguration
{
    public string BaseUrl { get; set; } = "https://api.bancaalfa.it";
    public string P2PTransferEndpoint { get; set; } = "/v1/payments/p2p-transfer";
    public int TimeoutSeconds { get; set; } = 30;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Abilita il mocking del servizio per test/sviluppo
    /// </summary>
    public bool EnableMocking { get; set; } = false;
    
    /// <summary>
    /// Configurazione dei mock scenarios
    /// </summary>
    public MockScenariosConfiguration MockScenarios { get; set; } = new();
}

public class MockScenariosConfiguration
{
    /// <summary>
    /// Scenario predefinito da utilizzare se non specificato
    /// </summary>
    public string DefaultScenario { get; set; } = "success";
    
    /// <summary>
    /// Configurazione dei vari scenarios di mock
    /// </summary>
    public Dictionary<string, MockScenario> Scenarios { get; set; } = new();
}

public class MockScenario
{
    /// <summary>
    /// Se true, simula un successo, altrimenti un errore
    /// </summary>
    public bool Success { get; set; } = true;
    
    /// <summary>
    /// Messaggio di errore (usato se Success = false)
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Codice di errore (usato se Success = false)
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Delay in millisecondi per simulare latenza
    /// </summary>
    public int DelayMs { get; set; } = 0;
    
    /// <summary>
    /// ID transazione di mock da restituire
    /// </summary>
    public string? MockTransactionId { get; set; }
}
