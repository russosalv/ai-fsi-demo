using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Banking.Infrastructure.Services;

/// <summary>
/// Implementazione mock del servizio P2P per test e sviluppo
/// </summary>
public class BancaAlfaP2PMockService : IBancaAlfaP2PService
{
    private readonly BancaAlfaApiConfiguration _config;
    private readonly ILogger<BancaAlfaP2PMockService> _logger;

    public BancaAlfaP2PMockService(
        IOptions<BancaAlfaApiConfiguration> config,
        ILogger<BancaAlfaP2PMockService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }    public async Task<P2PTransferResponse> TransferAsync(P2PTransferRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock P2P Transfer - From: {SenderTaxId}, To: {RecipientTaxId}, Amount: {Amount}", 
            request.SenderTaxId, request.RecipientTaxId, request.Amount);

        // Determina lo scenario da utilizzare
        var scenario = GetScenarioForRequest(request);
        
        // Simula delay se configurato
        if (scenario.DelayMs > 0)
        {
            await Task.Delay(scenario.DelayMs, cancellationToken);
        }

        // Simula successo o errore in base allo scenario
        if (!scenario.Success)
        {
            _logger.LogWarning("Mock P2P Transfer failed - Scenario: {Scenario}, Error: {Error}", 
                GetScenarioName(request), scenario.ErrorMessage);
            
            // In caso di errore, il servizio reale probabilmente lancerebbe un'eccezione
            // o restituirebbe una risposta con status diverso da "completed"
            return new P2PTransferResponse
            {
                Status = "failed",
                TransactionId = scenario.MockTransactionId ?? Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Amount = request.Amount,
                Currency = request.Currency,
                ExecutionDate = DateTime.UtcNow,
                ReferenceId = request.ReferenceId
            };
        }

        var transactionId = scenario.MockTransactionId ?? Guid.NewGuid().ToString();
        
        _logger.LogInformation("Mock P2P Transfer successful - TransactionId: {TransactionId}", transactionId);
        
        return new P2PTransferResponse
        {
            Status = "completed",
            TransactionId = transactionId,
            Timestamp = DateTime.UtcNow,
            Amount = request.Amount,
            Currency = request.Currency,
            ExecutionDate = DateTime.UtcNow,
            ReferenceId = request.ReferenceId,
            Sender = new ParticipantInfo { TaxId = request.SenderTaxId },
            Recipient = new ParticipantInfo { TaxId = request.RecipientTaxId }
        };
    }    public async Task<List<string>> ValidateRequestAsync(P2PTransferRequest request)
    {
        _logger.LogInformation("Mock P2P Validation - From: {SenderTaxId}, To: {RecipientTaxId}", 
            request.SenderTaxId, request.RecipientTaxId);

        var errors = new List<string>();
        
        // Determina lo scenario per la validazione
        var scenario = GetScenarioForRequest(request);
        
        // Simula delay se configurato
        if (scenario.DelayMs > 0)
        {
            await Task.Delay(Math.Min(scenario.DelayMs / 2, 1000)); // Max 1 secondo per validazione
        }

        // Validazioni di base (sempre attive)
        if (string.IsNullOrWhiteSpace(request.SenderTaxId))
            errors.Add("Tax ID mittente obbligatorio");
            
        if (string.IsNullOrWhiteSpace(request.RecipientTaxId))
            errors.Add("Tax ID destinatario obbligatorio");
            
        if (request.Amount <= 0)
            errors.Add("Importo deve essere maggiore di zero");

        // Se lo scenario prevede un errore di validazione, aggiungi errori specifici
        if (!scenario.Success && scenario.ErrorCode?.StartsWith("VALIDATION") == true)
        {
            errors.Add(scenario.ErrorMessage ?? "Errore di validazione mock");
        }

        return errors;
    }

    private MockScenario GetScenarioForRequest(P2PTransferRequest request)
    {
        var scenarioName = GetScenarioName(request);
        
        if (_config.MockScenarios.Scenarios.TryGetValue(scenarioName, out var scenario))
        {
            return scenario;
        }

        // Fallback al scenario di default
        if (_config.MockScenarios.Scenarios.TryGetValue(_config.MockScenarios.DefaultScenario, out var defaultScenario))
        {
            return defaultScenario;
        }

        // Fallback finale - successo
        return new MockScenario { Success = true };
    }    private string GetScenarioName(P2PTransferRequest request)
    {
        // Logica per determinare lo scenario basata sui dati della richiesta
        // Puoi personalizzare questa logica in base alle tue esigenze
        
        // Esempio: utilizza l'ultimo carattere del tax ID per determinare lo scenario
        if (!string.IsNullOrEmpty(request.RecipientTaxId))
        {
            var lastChar = char.ToLowerInvariant(request.RecipientTaxId.Last());
            return lastChar switch
            {
                't' => "error_insufficient_funds",
                'u' => "error_account_blocked",
                'v' => "error_invalid_account",
                'z' => "slow_response",
                _ => _config.MockScenarios.DefaultScenario
            };
        }

        return _config.MockScenarios.DefaultScenario;
    }
}
