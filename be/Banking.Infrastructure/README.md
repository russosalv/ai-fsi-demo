# Banking.Infrastructure

Questo progetto implementa la comunicazione con l'API P2P Banking di Banca Alfa secondo le specifiche tecniche del documento `api_p2p_banking_spec.md`.

## Panoramica

Il progetto Banking.Infrastructure fornisce:

- **Client HTTP** per la comunicazione con l'API di Banca Alfa
- **Gestione robusta degli errori** con mapping automatico dei codici di errore
- **Validazione dei dati** in input secondo le specifiche dell'API
- **Retry policy** configurabile per le chiamate HTTP
- **Logging** dettagliato per monitoraggio e troubleshooting
- **Configurazione flessibile** tramite appsettings.json

## Struttura del Progetto

```
Banking.Infrastructure/
├── Configuration/
│   └── BancaAlfaApiConfiguration.cs    # Configurazione dell'API
├── Constants/
│   └── BancaAlfaConstants.cs           # Costanti e limiti operativi
├── DTOs/
│   ├── Request/
│   │   └── P2PTransferRequest.cs       # DTO per la richiesta
│   └── Response/
│       ├── P2PTransferResponse.cs      # DTO per la risposta di successo
│       └── P2PTransferErrorResponse.cs # DTO per la risposta di errore
├── Exceptions/
│   └── BancaAlfaApiException.cs        # Eccezioni custom per gestione errori
├── Extensions/
│   └── ServiceCollectionExtensions.cs # Estensioni per dependency injection
├── Interfaces/
│   ├── IBancaAlfaApiClient.cs          # Interfaccia per il client API
│   └── IP2PTransferService.cs          # Interfaccia per il servizio P2P
└── Services/
    ├── BancaAlfaApiClient.cs           # Implementazione client HTTP
    └── P2PTransferService.cs           # Implementazione servizio P2P
```

## Configurazione

### appsettings.json

```json
{
  "BancaAlfaApi": {
    "BaseUrl": "https://api.bancaalfa.it",
    "TimeoutSeconds": 30,
    "ApiVersion": "v1",
    "EnableRetryPolicy": true,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 1000
  }
}
```

### appsettings.Development.json

```json
{
  "BancaAlfaApi": {
    "BaseUrl": "https://api-sandbox.bancaalfa.it",
    "EnableRetryPolicy": false
  }
}
```

## Utilizzo

### 1. Registrazione dei Servizi

Nel `Program.cs` dell'API:

```csharp
using Banking.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registra i servizi dell'Infrastructure
builder.Services.AddBancaAlfaInfrastructure(builder.Configuration);
```

### 2. Iniezione del Servizio

```csharp
public class P2PTransfersController : ControllerBase
{
    private readonly IP2PTransferService _p2pTransferService;

    public P2PTransfersController(IP2PTransferService p2pTransferService)
    {
        _p2pTransferService = p2pTransferService;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteTransfer([FromBody] P2PTransferRequestDto request)
    {
        var result = await _p2pTransferService.ExecuteTransferAsync(
            request.SenderTaxId,
            request.RecipientTaxId,
            request.Amount,
            request.Description,
            request.ReferenceId);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
```

## Gestione degli Errori

Il sistema gestisce automaticamente tutti gli errori specificati nella documentazione dell'API:

### Errori di Validazione (HTTP 400)
- `INVALID_TAX_ID`: Codice fiscale non valido
- `INVALID_AMOUNT`: Importo non valido
- `MISSING_REQUIRED_FIELD`: Campo obbligatorio mancante
- `INVALID_CURRENCY`: Valuta non supportata
- `DESCRIPTION_TOO_LONG`: Descrizione troppo lunga

### Errori di Business Logic (HTTP 422)
- `SENDER_NOT_FOUND`: Mittente non trovato
- `RECIPIENT_NOT_FOUND`: Destinatario non trovato
- `INSUFFICIENT_FUNDS`: Fondi insufficienti
- `ACCOUNT_BLOCKED`: Conto bloccato
- `SAME_ACCOUNT_TRANSFER`: Trasferimento stesso conto
- `DAILY_LIMIT_EXCEEDED`: Limite giornaliero superato
- `DUPLICATE_REFERENCE`: Riferimento duplicato

### Errori di Sistema (HTTP 500)
- `SYSTEM_ERROR`: Errore di sistema
- `TIMEOUT_ERROR`: Timeout transazione

## Validazione dei Dati

Il servizio implementa tutte le validazioni richieste dalle specifiche:

- **Codice Fiscale**: Formato italiano a 16 caratteri alfanumerici
- **Importo**: Tra 0.01€ e 5.000€ con massimo 2 decimali
- **Valuta**: Solo EUR supportata
- **Descrizione**: Massimo 140 caratteri
- **Reference ID**: Massimo 50 caratteri alfanumerici
- **Controllo stesso account**: Impedisce trasferimenti verso se stessi

## Limiti Operativi

- **Importo minimo**: 0.01€
- **Importo massimo per transazione**: 5.000€
- **Limite giornaliero per mittente**: 10.000€
- **Numero massimo transazioni/giorno**: 50 per mittente
- **Timeout richieste**: 30 secondi
- **Idempotenza**: Supportata tramite reference_id (24 ore)

## Logging

Il sistema registra:

- Richieste in arrivo con parametri principali
- Chiamate all'API esterna
- Errori e eccezioni
- Tentativi di retry
- Transazioni completate con successo

## Retry Policy

La retry policy è configurabile e implementa:

- **Exponential backoff**: Ritardo crescente tra i tentativi
- **Transient error handling**: Gestione automatica degli errori temporanei
- **Configurabile**: Numero massimo di tentativi e ritardo iniziale

## Sicurezza

- **HTTPS**: Tutte le comunicazioni tramite TLS 1.3
- **Timeout**: Prevenzione di chiamate infinite
- **Validazione input**: Controllo rigoroso dei parametri
- **Error handling**: Nessuna esposizione di informazioni sensibili

## Dipendenze

- `Microsoft.Extensions.Http` - HttpClient factory
- `Microsoft.Extensions.Configuration` - Configurazione
- `Microsoft.Extensions.Logging` - Logging
- `System.Text.Json` - Serializzazione JSON
- `Polly` - Retry policy e resilienza
- `Banking.Models` - Modelli del dominio

## Testing

Per testare il sistema:

1. Configurare l'URL sandbox nell'ambiente di sviluppo
2. Utilizzare codici fiscali di test forniti da Banca Alfa
3. Verificare i vari scenari di errore
4. Testare i limiti operativi

## Monitoraggio

Il sistema è pronto per il monitoraggio attraverso:

- **Logging strutturato**: Tutti gli eventi sono loggati con livelli appropriati
- **Metriche**: Tempi di risposta, errori, success rate
- **Health checks**: Possibilità di aggiungere health checks per l'API esterna
- **Tracing**: Supporto per distributed tracing se necessario
