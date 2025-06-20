using Banking.Infrastructure.DTOs.Request;
using Banking.Infrastructure.DTOs.Response;
using Banking.Infrastructure.Exceptions;
using Banking.Infrastructure.Interfaces;
using Banking.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Banking.Infrastructure.Services;

public class P2PTransferService : IP2PTransferService
{
    private readonly IBancaAlfaApiClient _apiClient;
    private readonly ILogger<P2PTransferService> _logger;

    // Regex per validazione codice fiscale italiano
    private static readonly Regex TaxIdRegex = new(
        @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$", 
        RegexOptions.Compiled);

    // Limiti operativi secondo le specifiche
    private const decimal MinAmount = 0.01m;
    private const decimal MaxAmount = 5000.00m;
    private const decimal DailyLimit = 10000.00m;
    private const int MaxDailyTransactions = 50;
    private const int MaxDescriptionLength = 140;
    private const int MaxReferenceIdLength = 50;

    public P2PTransferService(IBancaAlfaApiClient apiClient, ILogger<P2PTransferService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<P2PTransferResult> ExecuteTransferAsync(
        string senderTaxId, 
        string recipientTaxId, 
        decimal amount, 
        string? description = null, 
        string? referenceId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting P2P transfer from {SenderTaxId} to {RecipientTaxId} for amount {Amount}", 
                senderTaxId, recipientTaxId, amount);

            var request = new P2PTransferRequest
            {
                SenderTaxId = senderTaxId,
                RecipientTaxId = recipientTaxId,
                Amount = amount,
                Currency = "EUR",
                Description = description,
                ReferenceId = referenceId
            };

            // Validazione input
            var validationErrors = await ValidateTransferRequestAsync(request);
            if (validationErrors.Any())
            {
                var errorMessage = string.Join("; ", validationErrors);
                _logger.LogWarning("Validation failed for P2P transfer: {Errors}", errorMessage);
                
                return new P2PTransferResult
                {
                    IsSuccess = false,
                    ErrorCode = "VALIDATION_ERROR",
                    ErrorMessage = errorMessage,
                    ReferenceId = referenceId
                };
            }

            // Esecuzione trasferimento
            var response = await _apiClient.ExecuteP2PTransferAsync(request, cancellationToken);

            var result = new P2PTransferResult
            {
                IsSuccess = true,
                TransactionId = response.TransactionId,
                Timestamp = response.Timestamp,
                Amount = response.Amount,
                Currency = response.Currency,
                SenderTaxId = response.Sender.TaxId,
                SenderIban = response.Sender.AccountIban,
                RecipientTaxId = response.Recipient.TaxId,
                RecipientIban = response.Recipient.AccountIban,
                FeeAmount = response.Fees.Amount,
                ExecutionDate = response.ExecutionDate,
                ReferenceId = response.ReferenceId
            };

            _logger.LogInformation("P2P transfer completed successfully. Transaction ID: {TransactionId}", 
                result.TransactionId);

            return result;
        }
        catch (BancaAlfaApiException ex)
        {
            _logger.LogError(ex, "P2P transfer failed with API exception. Error code: {ErrorCode}", ex.ErrorCode);
            
            return new P2PTransferResult
            {
                IsSuccess = false,
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message,
                ReferenceId = referenceId,
                ErrorDetails = ex.Details
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during P2P transfer");
            
            return new P2PTransferResult
            {
                IsSuccess = false,
                ErrorCode = "SYSTEM_ERROR",
                ErrorMessage = "Errore temporaneo del sistema, riprovare più tardi",
                ReferenceId = referenceId
            };
        }
    }

    public async Task<List<string>> ValidateTransferRequestAsync(P2PTransferRequest request)
    {
        var errors = new List<string>();

        // Validazione campi obbligatori
        if (string.IsNullOrWhiteSpace(request.SenderTaxId))
        {
            errors.Add("Il codice fiscale del mittente è obbligatorio");
        }
        else if (!IsValidTaxId(request.SenderTaxId))
        {
            errors.Add("Il formato del codice fiscale del mittente non è valido");
        }

        if (string.IsNullOrWhiteSpace(request.RecipientTaxId))
        {
            errors.Add("Il codice fiscale del destinatario è obbligatorio");
        }
        else if (!IsValidTaxId(request.RecipientTaxId))
        {
            errors.Add("Il formato del codice fiscale del destinatario non è valido");
        }

        // Controllo che non sia lo stesso account
        if (!string.IsNullOrWhiteSpace(request.SenderTaxId) && 
            !string.IsNullOrWhiteSpace(request.RecipientTaxId) &&
            request.SenderTaxId.Equals(request.RecipientTaxId, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Non è possibile effettuare trasferimenti verso il proprio conto");
        }

        // Validazione importo
        if (request.Amount <= 0)
        {
            errors.Add("L'importo deve essere maggiore di zero");
        }
        else if (request.Amount < MinAmount)
        {
            errors.Add($"L'importo minimo è {MinAmount:C}");
        }
        else if (request.Amount > MaxAmount)
        {
            errors.Add($"L'importo massimo è {MaxAmount:C}");
        }
        else if (Math.Round(request.Amount, 2) != request.Amount)
        {
            errors.Add("L'importo può avere al massimo 2 decimali");
        }

        // Validazione valuta
        if (!string.IsNullOrWhiteSpace(request.Currency) && request.Currency != "EUR")
        {
            errors.Add("Attualmente è supportata solo la valuta EUR");
        }

        // Validazione descrizione
        if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > MaxDescriptionLength)
        {
            errors.Add($"La descrizione non può superare i {MaxDescriptionLength} caratteri");
        }

        // Validazione reference ID
        if (!string.IsNullOrWhiteSpace(request.ReferenceId))
        {
            if (request.ReferenceId.Length > MaxReferenceIdLength)
            {
                errors.Add($"Il reference ID non può superare i {MaxReferenceIdLength} caratteri");
            }
            else if (!IsValidReferenceId(request.ReferenceId))
            {
                errors.Add("Il reference ID può contenere solo caratteri alfanumerici");
            }
        }

        return await Task.FromResult(errors);
    }

    private static bool IsValidTaxId(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId) || taxId.Length != 16)
            return false;

        return TaxIdRegex.IsMatch(taxId.ToUpperInvariant());
    }

    private static bool IsValidReferenceId(string referenceId)
    {
        return referenceId.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }
}
