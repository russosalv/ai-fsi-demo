# Mock Service Configuration - Banca Alfa P2P

Questo documento descrive come configurare e utilizzare il servizio mock per l'integrazione P2P con Banca Alfa.

## Abilitazione del Mock

Il mock può essere abilitato in due modi:

### 1. Tramite configurazione (appsettings.json)

```json
{
  "BancaAlfaApi": {
    "EnableMocking": true,
    "MockScenarios": {
      // configurazione scenari
    }
  }
}
```

### 2. Tramite parametro nel codice

```csharp
// Abilita esplicitamente il mock
services.AddBancaAlfaInfrastructure(configuration, enableMocking: true);

// Utilizza la configurazione da appsettings
services.AddBancaAlfaInfrastructure(configuration);
```

## Configurazione degli Scenari

Gli scenari mock sono configurati nella sezione `MockScenarios` dell'appsettings:

```json
{
  "BancaAlfaApi": {
    "EnableMocking": true,
    "MockScenarios": {
      "DefaultScenario": "success",
      "Scenarios": {
        "success": {
          "Success": true,
          "DelayMs": 500,
          "MockTransactionId": "TXN-SUCCESS-001"
        },
        "error_insufficient_funds": {
          "Success": false,
          "ErrorCode": "INSUFFICIENT_FUNDS",
          "ErrorMessage": "Fondi insufficienti sul conto mittente",
          "DelayMs": 300
        }
      }
    }
  }
}
```

### Proprietà degli Scenari

- `Success` (bool): Se true simula un successo, se false un errore
- `ErrorCode` (string): Codice errore personalizzato (usato se Success = false)
- `ErrorMessage` (string): Messaggio di errore (usato se Success = false)
- `DelayMs` (int): Ritardo in millisecondi per simulare latenza di rete
- `MockTransactionId` (string): ID transazione personalizzato da restituire

## Selezione dello Scenario

Lo scenario viene selezionato automaticamente in base all'ultimo carattere del `RecipientTaxId` (case-insensitive):

- Carattere 't' o 'T': `error_insufficient_funds`
- Carattere 'u' o 'U': `error_account_blocked`
- Carattere 'v' o 'V': `error_invalid_account`
- Carattere 'z' o 'Z': `slow_response`
- Altri caratteri: `DefaultScenario`

### Esempi di Test

```csharp
// Test di successo
var request = new P2PTransferRequest 
{ 
    RecipientTaxId = "ABCDEF12345678900", // ultimo carattere '0' -> scenario default
    SenderTaxId = "SENDER123456789",
    Amount = 100.00m
};

// Test di errore fondi insufficienti
var request = new P2PTransferRequest 
{ 
    RecipientTaxId = "ABCDEF123456789T", // ultimo carattere 'T' -> error_insufficient_funds
    SenderTaxId = "SENDER123456789",
    Amount = 100.00m
};

// Test di account bloccato
var request = new P2PTransferRequest 
{ 
    RecipientTaxId = "ABCDEF123456789U", // ultimo carattere 'U' -> error_account_blocked
    SenderTaxId = "SENDER123456789",
    Amount = 100.00m
};

// Test di account non valido
var request = new P2PTransferRequest 
{ 
    RecipientTaxId = "ABCDEF123456789V", // ultimo carattere 'V' -> error_invalid_account
    SenderTaxId = "SENDER123456789",
    Amount = 100.00m
};

// Test di risposta lenta
var request = new P2PTransferRequest 
{ 
    RecipientTaxId = "ABCDEF123456789Z", // ultimo carattere 'Z' -> slow_response
    SenderTaxId = "SENDER123456789",
    Amount = 100.00m
};
```

## Scenari Predefiniti in Development

Nel file `appsettings.Development.json` sono configurati i seguenti scenari:

1. **success**: Transazione completata con successo (delay 500ms)
2. **error_insufficient_funds**: Errore fondi insufficienti (delay 300ms)
3. **error_account_blocked**: Account destinatario bloccato (delay 200ms)
4. **error_invalid_account**: Tax ID non valido (delay 100ms)
5. **slow_response**: Risposta lenta ma di successo (delay 3000ms)

## Logging

Il servizio mock logga tutte le operazioni con livello `Information` per i successi e `Warning` per gli errori, includendo:

- Parametri della richiesta (SenderTaxId, RecipientTaxId, Amount)
- Scenario utilizzato
- Transaction ID generato
- Eventuali errori simulati

## Utilizzo nei Test

Il mock service può essere utilizzato nei test di integrazione per verificare il comportamento dell'applicazione con diversi scenari di risposta dall'API di Banca Alfa, senza dover effettuare chiamate reali all'API esterna.

```csharp
[Test]
public async Task TestP2PTransfer_InsufficientFunds()
{
    // Arrange
    var request = new P2PTransferRequest 
    { 
        RecipientTaxId = "TEST123456789T", // Triggera scenario error_insufficient_funds
        SenderTaxId = "SENDER123456789",
        Amount = 1000.00m
    };

    // Act
    var response = await _p2pService.TransferAsync(request);

    // Assert
    Assert.AreEqual("failed", response.Status);
}
```
