# Banking.Infrastructure

Questo progetto implementa la comunicazione con l'API P2P Banking di Banca Alfa secondo le specifiche tecniche fornite.

## Caratteristiche

- **Comunicazione HTTP**: Client HTTP configurabile per l'API Banca Alfa
- **Gestione errori completa**: Mapping di tutti i codici di errore specificati (400, 422, 500)
- **Validazione locale**: Validazione dei dati prima dell'invio all'API esterna
- **Logging strutturato**: Log dettagliati per debugging e monitoraggio
- **Configurazione flessibile**: Configurazione tramite appsettings.json
- **Dependency Injection**: Integrazione con DI container di .NET

## Struttura del Progetto

```
Banking.Infrastructure/
├── Configuration/          # Configurazioni API
├── Constants/             # Costanti e limiti
├── Exceptions/           # Eccezioni custom per errori API
├── Extensions/           # Extension methods per DI
├── Interfaces/           # Contratti dei servizi
├── Models/              # DTOs per request/response
│   ├── Requests/        # Modelli per richieste
│   └── Responses/       # Modelli per risposte
├── Services/            # Implementazioni concrete
└── Utilities/           # Utilities per validazioni
```

## Configurazione

Aggiungi la seguente sezione nel tuo `appsettings.json`:

```json
{
  "BancaAlfa": {
    "BaseUrl": "https://api.bancaalfa.it",
    "P2PTransferEndpoint": "/v1/payments/p2p-transfer",
    "TimeoutSeconds": 30,
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

## Registrazione Servizi

Nel `Program.cs` o `Startup.cs`:

```csharp
using Banking.Infrastructure.Extensions;

// Aggiungi i servizi dell'infrastruttura
builder.Services.AddBancaAlfaInfrastructure(builder.Configuration);
```

## Utilizzo

### Esempio di utilizzo del servizio P2P

```csharp
public class PaymentController : ControllerBase
{
    private readonly IBancaAlfaP2PService _p2pService;

    public PaymentController(IBancaAlfaP2PService p2pService)
    {
        _p2pService = p2pService;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] P2PTransferRequest request)
    {
        try
        {
            // Validazione locale (opzionale, viene fatta anche nel servizio)
            var validationErrors = await _p2pService.ValidateRequestAsync(request);
            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            // Esecuzione trasferimento
            var result = await _p2pService.TransferAsync(request);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.ErrorCode, message = ex.Message });
        }
        catch (BusinessLogicException ex)
        {
            return UnprocessableEntity(new { error = ex.ErrorCode, message = ex.Message });
        }
        catch (SystemException ex)
        {
            return StatusCode(500, new { error = ex.ErrorCode, message = ex.Message });
        }
    }
}
```

## Gestione Errori

Il servizio mappa automaticamente tutti i codici di errore specificati dalla documentazione API:

### Errori di Validazione (HTTP 400)
- `INVALID_TAX_ID`: Formato codice fiscale non valido
- `INVALID_AMOUNT`: Importo fuori dai limiti (0.01€ - 5000.00€)
- `MISSING_REQUIRED_FIELD`: Campi obbligatori mancanti
- `INVALID_CURRENCY`: Valuta non supportata (solo EUR)
- `DESCRIPTION_TOO_LONG`: Descrizione troppo lunga (max 140 caratteri)

### Errori di Business Logic (HTTP 422)
- `SENDER_NOT_FOUND`: Mittente non trovato
- `RECIPIENT_NOT_FOUND`: Destinatario non trovato
- `INSUFFICIENT_FUNDS`: Fondi insufficienti
- `ACCOUNT_BLOCKED`: Conto bloccato
- `SAME_ACCOUNT_TRANSFER`: Trasferimento stesso conto
- `DAILY_LIMIT_EXCEEDED`: Limite giornaliero superato (10.000€)
- `DUPLICATE_REFERENCE`: Reference ID duplicato

### Errori di Sistema (HTTP 500)
- `SYSTEM_ERROR`: Errore temporaneo del sistema
- `TIMEOUT_ERROR`: Timeout transazione

## Validazioni Implementate

- **Codice Fiscale**: Regex per formato italiano (16 caratteri)
- **Importo**: Range 0.01€ - 5000.00€
- **Valuta**: Solo EUR supportata
- **Descrizione**: Max 140 caratteri
- **Reference ID**: Max 50 caratteri alfanumerici
- **Stessi Codici Fiscali**: Controllo mittente ≠ destinatario

## Limitazioni e Vincoli

Come da specifiche API:
- Importo min/max: 0.01€ - 5000.00€
- Limite giornaliero: 10.000€ per mittente
- Max transazioni/giorno: 50 per mittente
- Timeout richiesta: 30 secondi
- Formato date: ISO 8601 (UTC)
- Idempotenza: Supportata tramite reference_id (24 ore)

## Dependencies

- **Microsoft.Extensions.Http**: Per HttpClient factory
- **Microsoft.Extensions.Configuration**: Per configurazione
- **Microsoft.Extensions.DependencyInjection**: Per DI container
- **Microsoft.Extensions.Logging**: Per logging
- **System.Text.Json**: Per serializzazione JSON
