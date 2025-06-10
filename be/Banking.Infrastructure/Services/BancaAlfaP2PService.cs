using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Constants;
using Banking.Infrastructure.Exceptions;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Models.Responses;
using Banking.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Banking.Infrastructure.Services;

public class BancaAlfaP2PService : IBancaAlfaP2PService
{
    private readonly HttpClient _httpClient;
    private readonly BancaAlfaApiConfiguration _configuration;
    private readonly ILogger<BancaAlfaP2PService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // Regex per validazione codice fiscale italiano
    private static readonly Regex TaxIdRegex = new(
        @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
        RegexOptions.Compiled);

    public BancaAlfaP2PService(
        HttpClient httpClient,
        IOptions<BancaAlfaApiConfiguration> configuration,
        ILogger<BancaAlfaP2PService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration.Value;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
        
        // Aggiungi headers di autenticazione se configurati
        if (!string.IsNullOrEmpty(_configuration.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _configuration.ApiKey);
        }

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Banking.Infrastructure/1.0");
    }

    public async Task<P2PTransferResponse> TransferAsync(P2PTransferRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Avvio trasferimento P2P per riferimento {ReferenceId}", request.ReferenceId);

        try
        {
            // Validazione locale prima dell'invio
            var validationErrors = await ValidateRequestAsync(request);
            if (validationErrors.Any())
            {
                _logger.LogWarning("Validazione fallita per riferimento {ReferenceId}: {Errors}", 
                    request.ReferenceId, string.Join(", ", validationErrors));
                throw new ValidationErrors.MissingRequiredFieldException(DateTime.UtcNow, request.ReferenceId);
            }

            var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogDebug("Invio richiesta P2P a {Endpoint}", _configuration.P2PTransferEndpoint);

            var response = await _httpClient.PostAsync(_configuration.P2PTransferEndpoint, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var successResponse = JsonSerializer.Deserialize<P2PTransferResponse>(responseContent, _jsonOptions);
                if (successResponse == null)
                {
                    throw new SystemErrors.BancaAlfaSystemException(DateTime.UtcNow, request.ReferenceId);
                }

                _logger.LogInformation("Trasferimento P2P completato con successo. TransactionId: {TransactionId}", 
                    successResponse.TransactionId);

                return successResponse;
            }
            else
            {
                await HandleErrorResponseAsync(response, responseContent, request.ReferenceId);
                throw new SystemErrors.BancaAlfaSystemException(DateTime.UtcNow, request.ReferenceId);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout durante trasferimento P2P per riferimento {ReferenceId}", request.ReferenceId);
            throw new SystemErrors.TimeoutException(DateTime.UtcNow, request.ReferenceId);
        }
        catch (BancaAlfaApiException)
        {
            throw; // Re-throw specific exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante trasferimento P2P per riferimento {ReferenceId}", request.ReferenceId);
            throw new SystemErrors.BancaAlfaSystemException(DateTime.UtcNow, request.ReferenceId);
        }
    }    public Task<List<string>> ValidateRequestAsync(P2PTransferRequest request)
    {
        var errors = new List<string>();

        // Normalizza i dati prima della validazione
        request.SenderTaxId = ValidationUtilities.NormalizeTaxId(request.SenderTaxId) ?? string.Empty;
        request.RecipientTaxId = ValidationUtilities.NormalizeTaxId(request.RecipientTaxId) ?? string.Empty;
        request.Currency = ValidationUtilities.NormalizeCurrency(request.Currency);

        // Validazione campi obbligatori
        if (string.IsNullOrWhiteSpace(request.SenderTaxId))
            errors.Add("sender_tax_id è obbligatorio");

        if (string.IsNullOrWhiteSpace(request.RecipientTaxId))
            errors.Add("recipient_tax_id è obbligatorio");

        if (request.Amount <= 0)
            errors.Add("amount deve essere maggiore di 0");

        // Validazione formato codici fiscali usando utility
        if (!string.IsNullOrWhiteSpace(request.SenderTaxId) && !ValidationUtilities.IsValidTaxId(request.SenderTaxId))
            errors.Add("sender_tax_id non ha un formato valido");

        if (!string.IsNullOrWhiteSpace(request.RecipientTaxId) && !ValidationUtilities.IsValidTaxId(request.RecipientTaxId))
            errors.Add("recipient_tax_id non ha un formato valido");

        // Validazione stesso codice fiscale
        if (!string.IsNullOrWhiteSpace(request.SenderTaxId) && 
            !string.IsNullOrWhiteSpace(request.RecipientTaxId) &&
            request.SenderTaxId.Equals(request.RecipientTaxId, StringComparison.OrdinalIgnoreCase))
            errors.Add("sender_tax_id e recipient_tax_id non possono essere uguali");

        // Validazione importo usando utility
        if (!ValidationUtilities.IsValidAmount(request.Amount))
            errors.Add($"amount deve essere compreso tra {BancaAlfaConstants.Limits.MinAmount:C} e {BancaAlfaConstants.Limits.MaxAmount:C}");

        // Validazione valuta usando utility
        if (!ValidationUtilities.IsValidCurrency(request.Currency))
            errors.Add("currency deve essere EUR");

        // Validazione descrizione usando utility
        if (!ValidationUtilities.IsValidDescription(request.Description))
            errors.Add($"description non può superare i {BancaAlfaConstants.Limits.MaxDescriptionLength} caratteri");

        // Validazione reference_id usando utility
        if (!ValidationUtilities.IsValidReferenceId(request.ReferenceId))
        {
            if (!string.IsNullOrWhiteSpace(request.ReferenceId))
            {
                if (request.ReferenceId.Length > BancaAlfaConstants.Limits.MaxReferenceIdLength)
                    errors.Add($"reference_id non può superare i {BancaAlfaConstants.Limits.MaxReferenceIdLength} caratteri");
                else
                    errors.Add("reference_id può contenere solo caratteri alfanumerici, underscore e trattini");
            }
        }

        return Task.FromResult(errors);
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, string responseContent, string? referenceId)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<P2PTransferErrorResponse>(responseContent, _jsonOptions);
            if (errorResponse == null)
            {
                _logger.LogError("Impossibile deserializzare risposta di errore da Banca Alfa");
                return;
            }

            _logger.LogWarning("Errore API Banca Alfa: {ErrorCode} - {ErrorMessage}", 
                errorResponse.ErrorCode, errorResponse.ErrorMessage);

            var timestamp = errorResponse.Timestamp;
            var details = errorResponse.Details;

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    ThrowValidationException(errorResponse.ErrorCode, timestamp, referenceId, details);
                    break;

                case HttpStatusCode.UnprocessableEntity:
                    ThrowBusinessLogicException(errorResponse.ErrorCode, timestamp, referenceId, details);
                    break;

                case HttpStatusCode.InternalServerError:
                    ThrowSystemException(errorResponse.ErrorCode, timestamp, referenceId, details);
                    break;

                default:
                    throw new SystemErrors.BancaAlfaSystemException(timestamp, referenceId);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Errore durante parsing della risposta di errore da Banca Alfa");
        }
    }

    private static void ThrowValidationException(string errorCode, DateTime timestamp, string? referenceId, Dictionary<string, object>? details)
    {
        switch (errorCode)
        {
            case "INVALID_TAX_ID":
                throw new ValidationErrors.InvalidTaxIdException(timestamp, referenceId);
            case "INVALID_AMOUNT":
                throw new ValidationErrors.InvalidAmountException(timestamp, referenceId, details);
            case "MISSING_REQUIRED_FIELD":
                throw new ValidationErrors.MissingRequiredFieldException(timestamp, referenceId, details);
            case "INVALID_CURRENCY":
                throw new ValidationErrors.InvalidCurrencyException(timestamp, referenceId);
            case "DESCRIPTION_TOO_LONG":
                throw new ValidationErrors.DescriptionTooLongException(timestamp, referenceId);
            default:
                throw new ValidationException(errorCode, "Errore di validazione", timestamp, referenceId, details);
        }
    }

    private static void ThrowBusinessLogicException(string errorCode, DateTime timestamp, string? referenceId, Dictionary<string, object>? details)
    {
        switch (errorCode)
        {
            case "SENDER_NOT_FOUND":
                throw new BusinessLogicErrors.SenderNotFoundException(timestamp, referenceId);
            case "RECIPIENT_NOT_FOUND":
                throw new BusinessLogicErrors.RecipientNotFoundException(timestamp, referenceId);
            case "INSUFFICIENT_FUNDS":
                throw new BusinessLogicErrors.InsufficientFundsException(timestamp, referenceId, details);
            case "ACCOUNT_BLOCKED":
                throw new BusinessLogicErrors.AccountBlockedException(timestamp, referenceId);
            case "SAME_ACCOUNT_TRANSFER":
                throw new BusinessLogicErrors.SameAccountTransferException(timestamp, referenceId);
            case "DAILY_LIMIT_EXCEEDED":
                throw new BusinessLogicErrors.DailyLimitExceededException(timestamp, referenceId, details);
            case "DUPLICATE_REFERENCE":
                throw new BusinessLogicErrors.DuplicateReferenceException(timestamp, referenceId);
            default:
                throw new BusinessLogicException(errorCode, "Errore di business logic", timestamp, referenceId, details);
        }
    }    private static void ThrowSystemException(string errorCode, DateTime timestamp, string? referenceId, Dictionary<string, object>? details)
    {
        switch (errorCode)
        {
            case "SYSTEM_ERROR":
                throw new SystemErrors.BancaAlfaSystemException(timestamp, referenceId);
            case "TIMEOUT_ERROR":
                throw new SystemErrors.TimeoutException(timestamp, referenceId);
            default:
                throw new Exceptions.SystemException(errorCode, "Errore di sistema", timestamp, referenceId, details);
        }
    }
}
