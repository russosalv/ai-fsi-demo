# Integrazione Banking.Infrastructure nel progetto API

## Aggiungere il Reference al Progetto

1. Aggiungi il reference al progetto `Banking.Infrastructure` nel file `Banking.API.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Banking.Models\Banking.Models.csproj" />
  <ProjectReference Include="..\Banking.Logic\Banking.Logic.csproj" />
  <ProjectReference Include="..\Banking.Infrastructure\Banking.Infrastructure.csproj" />
</ItemGroup>
```

## Configurazione nel Program.cs

2. Nel file `Program.cs`, aggiungi la configurazione per i servizi dell'infrastruttura:

```csharp
using Banking.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ... altri servizi ...

// Aggiungi i servizi dell'infrastruttura Banca Alfa
builder.Services.AddBancaAlfaInfrastructure(builder.Configuration);

var app = builder.Build();
```

## Configurazione appsettings.json

3. Aggiungi la configurazione nel file `appsettings.json`:

```json
{
  "BancaAlfa": {
    "BaseUrl": "https://api.bancaalfa.it",
    "P2PTransferEndpoint": "/v1/payments/p2p-transfer",
    "TimeoutSeconds": 30,
    "ApiKey": "your-api-key-here",
    "ApiSecret": "your-api-secret-here"
  }
}
```

## Esempio di Controller P2P

4. Crea un nuovo controller per i trasferimenti P2P:

```csharp
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class P2PTransferController : ControllerBase
{
    private readonly IBancaAlfaP2PService _p2pService;
    private readonly ILogger<P2PTransferController> _logger;

    public P2PTransferController(
        IBancaAlfaP2PService p2pService,
        ILogger<P2PTransferController> logger)
    {
        _p2pService = p2pService;
        _logger = logger;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] P2PTransferRequest request)
    {
        try
        {
            _logger.LogInformation("Richiesta trasferimento P2P ricevuta per riferimento {ReferenceId}", 
                request.ReferenceId);

            // Validazione locale opzionale
            var validationErrors = await _p2pService.ValidateRequestAsync(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Validazione fallita: {Errors}", string.Join(", ", validationErrors));
                return BadRequest(new { errors = validationErrors });
            }

            // Esecuzione trasferimento
            var result = await _p2pService.TransferAsync(request);
            
            _logger.LogInformation("Trasferimento P2P completato con successo. TransactionId: {TransactionId}", 
                result.TransactionId);

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Errore di validazione: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
            return BadRequest(new 
            { 
                error = ex.ErrorCode, 
                message = ex.Message,
                timestamp = ex.Timestamp,
                reference_id = ex.ReferenceId
            });
        }
        catch (BusinessLogicException ex)
        {
            _logger.LogWarning("Errore di business logic: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
            return UnprocessableEntity(new 
            { 
                error = ex.ErrorCode, 
                message = ex.Message,
                timestamp = ex.Timestamp,
                reference_id = ex.ReferenceId,
                details = ex.Details
            });
        }
        catch (Exceptions.SystemException ex)
        {
            _logger.LogError("Errore di sistema: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
            return StatusCode(500, new 
            { 
                error = ex.ErrorCode, 
                message = ex.Message,
                timestamp = ex.Timestamp,
                reference_id = ex.ReferenceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante trasferimento P2P");
            return StatusCode(500, new 
            { 
                error = "INTERNAL_ERROR", 
                message = "Errore interno del server" 
            });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateTransfer([FromBody] P2PTransferRequest request)
    {
        try
        {
            var validationErrors = await _p2pService.ValidateRequestAsync(request);
            
            if (validationErrors.Any())
            {
                return BadRequest(new { errors = validationErrors, valid = false });
            }

            return Ok(new { valid = true, message = "Richiesta valida" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante validazione richiesta P2P");
            return StatusCode(500, new { error = "VALIDATION_ERROR", message = "Errore durante validazione" });
        }
    }
}
```

## Test dell'Integrazione

5. Per testare l'integrazione, puoi utilizzare i seguenti comandi curl:

```bash
# Validazione richiesta
curl -X POST "https://localhost:7071/api/P2PTransfer/validate" \
     -H "Content-Type: application/json" \
     -d '{
       "sender_tax_id": "RSSMRA85M01H501Z",
       "recipient_tax_id": "VRDRBT90A41F205X",
       "amount": 150.50,
       "currency": "EUR",
       "description": "Test transfer",
       "reference_id": "TEST_001"
     }'

# Esecuzione trasferimento
curl -X POST "https://localhost:7071/api/P2PTransfer/transfer" \
     -H "Content-Type: application/json" \
     -d '{
       "sender_tax_id": "RSSMRA85M01H501Z",
       "recipient_tax_id": "VRDRBT90A41F205X",
       "amount": 150.50,
       "currency": "EUR",
       "description": "Test transfer",
       "reference_id": "TEST_001"
     }'
```

## Note Importanti

- Assicurati di configurare correttamente le credenziali API in un ambiente sicuro
- Implementa logging appropriato per monitoraggio e debugging
- Considera l'implementazione di middleware per rate limiting e autenticazione
- Testa thoroughly in ambiente di development prima del deploy in produzione
