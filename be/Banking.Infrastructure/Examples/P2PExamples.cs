using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Models.Responses;

namespace Banking.Infrastructure.Examples;

/// <summary>
/// Esempi di utilizzo del servizio P2P Banking
/// </summary>
public static class P2PExamples
{
    /// <summary>
    /// Esempio di richiesta P2P valida
    /// </summary>
    public static P2PTransferRequest CreateValidRequest()
    {
        return new P2PTransferRequest
        {
            SenderTaxId = "RSSMRA85M01H501Z",
            RecipientTaxId = "VRDRBT90A41F205X",
            Amount = 150.50m,
            Currency = "EUR",
            Description = "Rimborso cena",
            ReferenceId = $"TXN_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid().ToString("N")[..8]}"
        };
    }

    /// <summary>
    /// Esempio di richiesta P2P con dati minimi
    /// </summary>
    public static P2PTransferRequest CreateMinimalRequest()
    {
        return new P2PTransferRequest
        {
            SenderTaxId = "RSSMRA85M01H501Z",
            RecipientTaxId = "VRDRBT90A41F205X",
            Amount = 0.01m
            // Currency, Description e ReferenceId sono opzionali
        };
    }

    /// <summary>
    /// Esempio di richiesta P2P con importo massimo
    /// </summary>
    public static P2PTransferRequest CreateMaxAmountRequest()
    {
        return new P2PTransferRequest
        {
            SenderTaxId = "RSSMRA85M01H501Z",
            RecipientTaxId = "VRDRBT90A41F205X",
            Amount = 5000.00m,
            Currency = "EUR",
            Description = "Pagamento massimo consentito",
            ReferenceId = "MAX_TRANSFER_001"
        };
    }

    /// <summary>
    /// Esempio di richiesta P2P non valida (per test di validazione)
    /// </summary>
    public static P2PTransferRequest CreateInvalidRequest()
    {
        return new P2PTransferRequest
        {
            SenderTaxId = "INVALID_TAX_ID",  // Formato non valido
            RecipientTaxId = "INVALID_TAX_ID", // Formato non valido + stesso del mittente
            Amount = 6000.00m, // Oltre il limite massimo
            Currency = "USD", // Valuta non supportata
            Description = "Questa descrizione è troppo lunga e supera il limite di 140 caratteri consentito dall'API di Banca Alfa per le causali dei pagamenti P2P",
            ReferenceId = "QUESTO_REFERENCE_ID_È_TROPPO_LUNGO_E_SUPERA_I_50_CARATTERI_CONSENTITI" // Troppo lungo
        };
    }

    /// <summary>
    /// Esempio di risposta di successo
    /// </summary>
    public static P2PTransferResponse CreateSuccessResponse()
    {
        return new P2PTransferResponse
        {
            Status = "success",
            TransactionId = "TXN_BA_20250610_123456789",
            Timestamp = DateTime.UtcNow,
            Amount = 150.50m,
            Currency = "EUR",
            Sender = new ParticipantInfo
            {
                TaxId = "RSSMRA85M01H501Z",
                AccountIban = "IT60X0542811101000000123456"
            },
            Recipient = new ParticipantInfo
            {
                TaxId = "VRDRBT90A41F205X",
                AccountIban = "IT60X0542811101000000654321"
            },
            Fees = new FeeInfo
            {
                Amount = 0.00m,
                Currency = "EUR"
            },
            ExecutionDate = DateTime.UtcNow,
            ReferenceId = "TXN_20250610_001"
        };
    }
}
