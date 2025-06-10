# Specifiche Tecniche API P2P Banking
## Banca Alfa - Core Banking System

**Versione:** 1.0  
**Data:** Giugno 2025  
**Autore:** Technical Leadership Team

---

## Panoramica

L'API P2P Banking di Banca Alfa consente di effettuare trasferimenti di denaro istantanei tra clienti della banca utilizzando il codice fiscale come identificativo univoco. Il servizio garantisce transazioni sicure, tracciabili e conformi alle normative PSD2.

## Endpoint Principal

### POST /api/v1/payments/p2p-transfer

Effettua un trasferimento di denaro peer-to-peer tra due clienti della banca.

**URL:** `https://api.bancaalfa.it/v1/payments/p2p-transfer`  
**Metodo:** `POST`  
**Content-Type:** `application/json`

## Parametri di Input

### Request Body (JSON)

```json
{
  "sender_tax_id": "string",
  "recipient_tax_id": "string", 
  "amount": "number",
  "currency": "string",
  "description": "string",
  "reference_id": "string"
}
```

### Descrizione Parametri

| Campo | Tipo | Obbligatorio | Descrizione |
|-------|------|--------------|-------------|
| `sender_tax_id` | string | Sì | Codice fiscale del mittente (16 caratteri alfanumerici) |
| `recipient_tax_id` | string | Sì | Codice fiscale del destinatario (16 caratteri alfanumerici) |
| `amount` | number | Sì | Importo in euro (max 2 decimali, min 0.01, max 5000.00) |
| `currency` | string | No | Codice valuta ISO 4217 (default: "EUR") |
| `description` | string | No | Causale del pagamento (max 140 caratteri) |
| `reference_id` | string | No | ID di riferimento del cliente (max 50 caratteri alfanumerici) |

### Esempio Request

```json
{
  "sender_tax_id": "RSSMRA85M01H501Z",
  "recipient_tax_id": "VRDRBT90A41F205X",
  "amount": 150.50,
  "currency": "EUR",
  "description": "Rimborso cena",
  "reference_id": "TXN_20250610_001"
}
```

## Risposta di Output

### Response Success (HTTP 200)

```json
{
  "status": "success",
  "transaction_id": "string",
  "timestamp": "string",
  "amount": "number",
  "currency": "string",
  "sender": {
    "tax_id": "string",
    "account_iban": "string"
  },
  "recipient": {
    "tax_id": "string", 
    "account_iban": "string"
  },
  "fees": {
    "amount": "number",
    "currency": "string"
  },
  "execution_date": "string",
  "reference_id": "string"
}
```

### Esempio Response Success

```json
{
  "status": "success",
  "transaction_id": "TXN_BA_20250610_123456789",
  "timestamp": "2025-06-10T14:30:25.123Z",
  "amount": 150.50,
  "currency": "EUR",
  "sender": {
    "tax_id": "RSSMRA85M01H501Z",
    "account_iban": "IT60X0542811101000000123456"
  },
  "recipient": {
    "tax_id": "VRDRBT90A41F205X",
    "account_iban": "IT60X0542811101000000654321"
  },
  "fees": {
    "amount": 0.00,
    "currency": "EUR"
  },
  "execution_date": "2025-06-10T14:30:25.123Z",
  "reference_id": "TXN_20250610_001"
}
```

## Codici di Errore e Casi Particolari

### Errori di Validazione (HTTP 400)

| Codice Errore | Descrizione | Dettaglio |
|---------------|-------------|-----------|
| `INVALID_TAX_ID` | Codice fiscale non valido | Il formato del codice fiscale non rispetta la normativa italiana |
| `INVALID_AMOUNT` | Importo non valido | L'importo deve essere compreso tra 0.01€ e 5000.00€ |
| `MISSING_REQUIRED_FIELD` | Campo obbligatorio mancante | Uno o più campi obbligatori non sono stati forniti |
| `INVALID_CURRENCY` | Valuta non supportata | Attualmente supportata solo EUR |
| `DESCRIPTION_TOO_LONG` | Descrizione troppo lunga | La causale non può superare i 140 caratteri |

### Errori di Business Logic (HTTP 422)

| Codice Errore | Descrizione | Dettaglio |
|---------------|-------------|-----------|
| `SENDER_NOT_FOUND` | Mittente non trovato | Il codice fiscale del mittente non è associato a nessun conto |
| `RECIPIENT_NOT_FOUND` | Destinatario non trovato | Il codice fiscale del destinatario non è associato a nessun conto |
| `INSUFFICIENT_FUNDS` | Fondi insufficienti | Il mittente non ha fondi sufficienti per completare la transazione |
| `ACCOUNT_BLOCKED` | Conto bloccato | Il conto del mittente o destinatario è temporaneamente bloccato |
| `SAME_ACCOUNT_TRANSFER` | Trasferimento stesso conto | Non è possibile effettuare trasferimenti verso il proprio conto |
| `DAILY_LIMIT_EXCEEDED` | Limite giornaliero superato | Superato il limite giornaliero di trasferimenti P2P (10.000€) |
| `DUPLICATE_REFERENCE` | Riferimento duplicato | Il reference_id è già stato utilizzato nelle ultime 24 ore |

### Errori di Sistema (HTTP 500)

| Codice Errore | Descrizione | Dettaglio |
|---------------|-------------|-----------|
| `SYSTEM_ERROR` | Errore di sistema | Errore temporaneo del sistema, riprovare più tardi |
| `TIMEOUT_ERROR` | Timeout transazione | La transazione ha impiegato troppo tempo, verificare lo stato |

### Esempio Response Errore

```json
{
  "status": "error",
  "error_code": "INSUFFICIENT_FUNDS",
  "error_message": "Fondi insufficienti per completare la transazione",
  "timestamp": "2025-06-10T14:30:25.123Z",
  "reference_id": "TXN_20250610_001",
  "details": {
    "available_balance": 120.00,
    "requested_amount": 150.50
  }
}
```

## Limitazioni e Vincoli

### Limiti Operativi
- **Importo minimo:** 0.01€
- **Importo massimo per transazione:** 5.000,00€
- **Limite giornaliero per mittente:** 10.000,00€
- **Numero massimo transazioni/giorno:** 50 per mittente

### Orari di Servizio
- **Disponibilità:** 24/7 (salvo manutenzioni programmate)
- **Esecuzione immediata:** Lun-Ven 08:00-20:00
- **Esecuzione differita:** Altri orari (esecuzione entro il giorno lavorativo successivo)

## Sicurezza e Compliance

### Tracciabilità
- Tutte le transazioni sono registrate e tracciabili
- Conformità alle normative PSD2 e antiriciclaggio
- Log di audit completi per ogni operazione

### Crittografia
- Comunicazioni tramite TLS 1.3
- Dati sensibili crittografati con AES-256
- Firma digitale delle transazioni con algoritmo RSA-4096

## Note Tecniche

- **Timeout richiesta:** 30 secondi
- **Formato date:** ISO 8601 (UTC)
- **Codifica caratteri:** UTF-8
- **Idempotenza:** Supportata tramite reference_id (24 ore)

## Contatti Tecnici

**Team API Banking**  
Email: api-support@bancaalfa.it  
Telefono: +39 02 1234 5678  
Documentazione: https://developer.bancaalfa.it

---

*Documento riservato - Banca Alfa S.p.A. - Tutti i diritti riservati*