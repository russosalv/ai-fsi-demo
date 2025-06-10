# Banking API - Integrazione P2P Banca Alfa

## Panoramica

Questo documento descrive l'integrazione del servizio P2P di Banca Alfa nel progetto Banking.API.

## Configurazione

### appsettings.json

```json
{
  "BancaAlfaApi": {
    "BaseUrl": "https://api.bancaalfa.it",
    "P2PTransferEndpoint": "/v1/payments/p2p-transfer",
    "TimeoutSeconds": 30,
    "ApiKey": "your-production-api-key",
    "ApiSecret": "your-production-api-secret"
  }
}
```

### appsettings.Development.json

```json
{
  "BancaAlfaApi": {
    "BaseUrl": "https://api-sandbox.bancaalfa.it",
    "P2PTransferEndpoint": "/v1/payments/p2p-transfer",
    "TimeoutSeconds": 30,
    "ApiKey": "demo-api-key",
    "ApiSecret": "demo-api-secret"
  }
}
```

## Endpoints

### 1. Health Check
- **URL**: `GET /api/P2P/health`
- **Descrizione**: Verifica lo stato del servizio P2P
- **Risposta**: 
  ```json
  {
    "status": "healthy",
    "service": "P2P Banking Service",
    "timestamp": "2025-06-10T10:30:00Z",
    "version": "1.0.0"
  }
  ```

### 2. Validazione Richiesta
- **URL**: `POST /api/P2P/validate`
- **Descrizione**: Valida una richiesta di trasferimento senza eseguirla
- **Body**:
  ```json
  {
    "sender_tax_id": "RSSMRA85M01H501U",
    "recipient_tax_id": "BNCGPP90A01F205X",
    "amount": 100.50,
    "currency": "EUR",
    "description": "Descrizione del pagamento",
    "reference_id": "REF-2025-001"
  }
  ```
- **Risposta Success**:
  ```json
  {
    "isValid": true,
    "errors": [],
    "timestamp": "2025-06-10T10:30:00Z",
    "referenceId": "REF-2025-001"
  }
  ```

### 3. Trasferimento P2P
- **URL**: `POST /api/P2P/transfer`
- **Descrizione**: Esegue un trasferimento P2P tra due clienti
- **Body**: Come per la validazione
- **Risposta Success**:
  ```json
  {
    "status": "completed",
    "transaction_id": "TXN-123456789",
    "timestamp": "2025-06-10T10:30:00Z",
    "amount": 100.50,
    "currency": "EUR",
    "sender": {
      "tax_id": "RSSMRA85M01H501U",
      "account_iban": "IT60X0542811101000000123456"
    },
    "recipient": {
      "tax_id": "BNCGPP90A01F205X",
      "account_iban": "IT28A0300203280123456789012"
    },
    "fees": {
      "amount": 1.50,
      "currency": "EUR"
    },
    "execution_date": "2025-06-10T10:30:00Z",
    "reference_id": "REF-2025-001"
  }
  ```

## Gestione Errori

### Errori di Validazione (400 Bad Request)
- `INVALID_TAX_ID`: Codice fiscale non valido
- `INVALID_AMOUNT`: Importo non valido
- `MISSING_REQUIRED_FIELD`: Campo obbligatorio mancante
- `INVALID_CURRENCY`: Valuta non supportata
- `DESCRIPTION_TOO_LONG`: Descrizione troppo lunga

### Errori di Business Logic (422 Unprocessable Entity)
- `SENDER_NOT_FOUND`: Mittente non trovato
- `RECIPIENT_NOT_FOUND`: Destinatario non trovato
- `INSUFFICIENT_FUNDS`: Fondi insufficienti
- `ACCOUNT_BLOCKED`: Account bloccato
- `SAME_ACCOUNT_TRANSFER`: Trasferimento sullo stesso account
- `DAILY_LIMIT_EXCEEDED`: Limite giornaliero superato
- `DUPLICATE_REFERENCE`: Riferimento duplicato

### Errori di Sistema (500 Internal Server Error)
- `SYSTEM_ERROR`: Errore generico del sistema
- `TIMEOUT_ERROR`: Timeout nella comunicazione
- `INTERNAL_ERROR`: Errore interno imprevisto

## Validazioni

### Codice Fiscale
- Formato italiano standard: `^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$`
- Normalizzazione automatica (rimozione spazi e conversione in maiuscolo)

### Importo
- Deve essere maggiore di 0
- Range valido: €0.01 - €50,000.00

### Valuta
- Solo EUR supportata

### Descrizione
- Massimo 200 caratteri
- Opzionale

### Reference ID
- Massimo 50 caratteri
- Solo caratteri alfanumerici, underscore e trattini
- Opzionale

## Sicurezza

- API Key e Secret configurabili per ambiente
- Timeout configurabile per evitare attese eccessive
- Logging dettagliato per audit e debugging
- Validazione rigorosa dei dati in input

## Logging

Il servizio registra:
- Richieste ricevute con Reference ID
- Validazioni eseguite
- Trasferimenti completati con Transaction ID
- Errori con dettagli per debugging
- Timeout e problemi di comunicazione

## Testing

Utilizzare il file `P2P.http` incluso per testare tutti gli endpoint con scenari positivi e negativi.

## Dipendenze

- Banking.Infrastructure: Servizi per comunicazione con Banca Alfa
- System.Text.Json: Serializzazione JSON
- Microsoft.Extensions.Http: HttpClient factory
- Microsoft.Extensions.Logging: Logging
