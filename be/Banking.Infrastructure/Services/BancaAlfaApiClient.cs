using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.DTOs.Request;
using Banking.Infrastructure.DTOs.Response;
using Banking.Infrastructure.Exceptions;
using Banking.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Banking.Infrastructure.Services;

public class BancaAlfaApiClient : IBancaAlfaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly BancaAlfaApiConfiguration _configuration;
    private readonly ILogger<BancaAlfaApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public BancaAlfaApiClient(
        HttpClient httpClient, 
        IOptions<BancaAlfaApiConfiguration> configuration,
        ILogger<BancaAlfaApiClient> logger)
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

    public async Task<P2PTransferResponse> ExecuteP2PTransferAsync(P2PTransferRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Initiating P2P transfer for reference ID: {ReferenceId}", request.ReferenceId);

            var endpoint = $"/{_configuration.ApiVersion}/payments/p2p-transfer";
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {                var successResponse = JsonSerializer.Deserialize<P2PTransferResponse>(responseContent, _jsonOptions);
                if (successResponse == null)
                {
                    throw new BancaAlfaSystemException("DESERIALIZATION_ERROR", "Failed to deserialize success response", request.ReferenceId);
                }

                _logger.LogInformation("P2P transfer completed successfully. Transaction ID: {TransactionId}", successResponse.TransactionId);
                return successResponse;
            }

            await HandleErrorResponseAsync(response, responseContent, request.ReferenceId);
              // This should never be reached due to exception throwing in HandleErrorResponseAsync
            throw new BancaAlfaSystemException("UNKNOWN_ERROR", "Unknown error occurred", request.ReferenceId);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout occurred during P2P transfer for reference ID: {ReferenceId}", request.ReferenceId);
            throw new BancaAlfaSystemException("TIMEOUT_ERROR", "La transazione ha impiegato troppo tempo, verificare lo stato", request.ReferenceId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred during P2P transfer for reference ID: {ReferenceId}", request.ReferenceId);
            throw new BancaAlfaSystemException("NETWORK_ERROR", "Errore di connessione al servizio", request.ReferenceId);
        }
        catch (BancaAlfaApiException)
        {
            // Re-throw our custom exceptions
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during P2P transfer for reference ID: {ReferenceId}", request.ReferenceId);
            throw new BancaAlfaSystemException("SYSTEM_ERROR", "Errore temporaneo del sistema, riprovare più tardi", request.ReferenceId);
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Banking.Infrastructure/1.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    private Task HandleErrorResponseAsync(HttpResponseMessage response, string responseContent, string? referenceId)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<P2PTransferErrorResponse>(responseContent, _jsonOptions);
            
            if (errorResponse != null)
            {
                _logger.LogWarning("API returned error. Status: {StatusCode}, Code: {ErrorCode}, Message: {ErrorMessage}",
                    response.StatusCode, errorResponse.ErrorCode, errorResponse.ErrorMessage);

                var exception = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new ValidationException(
                        errorResponse.ErrorCode, 
                        errorResponse.ErrorMessage, 
                        referenceId, 
                        errorResponse.Details),
                    
                    HttpStatusCode.UnprocessableEntity => new BusinessLogicException(
                        errorResponse.ErrorCode, 
                        errorResponse.ErrorMessage, 
                        referenceId, 
                        errorResponse.Details),
                      HttpStatusCode.InternalServerError => new BancaAlfaSystemException(
                        errorResponse.ErrorCode, 
                        errorResponse.ErrorMessage, 
                        referenceId, 
                        errorResponse.Details),
                    
                    _ => new BancaAlfaApiException(
                        errorResponse.ErrorCode ?? "UNKNOWN_ERROR", 
                        errorResponse.ErrorMessage ?? "Unknown error occurred", 
                        (int)response.StatusCode, 
                        referenceId, 
                        errorResponse.Details)
                };

                throw exception;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize error response. Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);
        }

        // Fallback if we can't parse the error response
        var genericMessage = response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Richiesta non valida",
            HttpStatusCode.UnprocessableEntity => "Errore nella logica di business",
            HttpStatusCode.InternalServerError => "Errore interno del server",
            _ => $"Errore HTTP {(int)response.StatusCode}"
        };

        throw new BancaAlfaApiException("HTTP_ERROR", genericMessage, (int)response.StatusCode, referenceId);
    }
}
