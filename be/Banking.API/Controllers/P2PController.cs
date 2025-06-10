using Microsoft.AspNetCore.Mvc;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Models.Responses;
using Banking.Infrastructure.Exceptions;

namespace Banking.API.Controllers;

/// <summary>
/// Controller per i trasferimenti P2P tramite Banca Alfa
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class P2PController : ControllerBase
{
    private readonly IBancaAlfaP2PService _p2pService;
    private readonly ILogger<P2PController> _logger;

    public P2PController(IBancaAlfaP2PService p2pService, ILogger<P2PController> logger)
    {
        _p2pService = p2pService;
        _logger = logger;
    }

    /// <summary>
    /// Effettua un trasferimento P2P tra due clienti della banca
    /// </summary>
    /// <param name="request">Dati del trasferimento</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Risposta del trasferimento</returns>
    /// <response code="200">Trasferimento completato con successo</response>
    /// <response code="400">Errore di validazione dei dati</response>
    /// <response code="422">Errore di business logic</response>
    /// <response code="500">Errore di sistema</response>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(P2PTransferResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BusinessLogicErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(SystemErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<P2PTransferResponse>> TransferAsync(
        [FromBody] P2PTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Richiesta trasferimento P2P ricevuta per riferimento {ReferenceId}", 
                request.ReferenceId);

            var response = await _p2pService.TransferAsync(request, cancellationToken);
            
            _logger.LogInformation("Trasferimento P2P completato con successo. TransactionId: {TransactionId}", 
                response.TransactionId);
            
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Errore di validazione per trasferimento P2P: {ErrorCode}", ex.ErrorCode);
            
            return BadRequest(new ValidationErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message,
                Timestamp = ex.Timestamp,
                ReferenceId = ex.ReferenceId,
                Details = ex.Details
            });
        }
        catch (BusinessLogicException ex)
        {
            _logger.LogWarning(ex, "Errore di business logic per trasferimento P2P: {ErrorCode}", ex.ErrorCode);
            
            return UnprocessableEntity(new BusinessLogicErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                ErrorMessage = ex.Message,
                Timestamp = ex.Timestamp,
                ReferenceId = ex.ReferenceId,
                Details = ex.Details
            });
        }        catch (Banking.Infrastructure.Exceptions.SystemException ex)
        {
            _logger.LogError(ex, "Errore di sistema per trasferimento P2P: {ErrorCode}", ex.ErrorCode);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new SystemErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                ErrorMessage = "Si è verificato un errore interno del sistema",
                Timestamp = ex.Timestamp,
                ReferenceId = ex.ReferenceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore imprevisto durante trasferimento P2P");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new SystemErrorResponse
            {
                ErrorCode = "INTERNAL_ERROR",
                ErrorMessage = "Si è verificato un errore interno imprevisto",
                Timestamp = DateTime.UtcNow,
                ReferenceId = request.ReferenceId
            });
        }
    }

    /// <summary>
    /// Valida una richiesta di trasferimento P2P senza eseguirla
    /// </summary>
    /// <param name="request">Richiesta da validare</param>
    /// <returns>Risultato della validazione</returns>
    /// <response code="200">Validazione completata</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidationResult>> ValidateAsync([FromBody] P2PTransferRequest request)
    {
        try
        {
            _logger.LogInformation("Richiesta validazione P2P ricevuta per riferimento {ReferenceId}", 
                request.ReferenceId);

            var errors = await _p2pService.ValidateRequestAsync(request);
            
            var result = new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors,
                Timestamp = DateTime.UtcNow,
                ReferenceId = request.ReferenceId
            };

            _logger.LogInformation("Validazione P2P completata. Valida: {IsValid}, Errori: {ErrorCount}", 
                result.IsValid, errors.Count);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante validazione richiesta P2P");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new SystemErrorResponse
            {
                ErrorCode = "VALIDATION_ERROR",
                ErrorMessage = "Errore durante la validazione della richiesta",
                Timestamp = DateTime.UtcNow,
                ReferenceId = request.ReferenceId
            });
        }
    }

    /// <summary>
    /// Ottiene lo stato di salute del servizio P2P
    /// </summary>
    /// <returns>Stato di salute del servizio</returns>
    /// <response code="200">Servizio attivo</response>
    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthCheckResponse> HealthCheck()
    {
        return Ok(new HealthCheckResponse
        {
            Status = "healthy",
            Service = "P2P Banking Service",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}

/// <summary>
/// Risposta di errore di validazione
/// </summary>
public class ValidationErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ReferenceId { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}

/// <summary>
/// Risposta di errore di business logic
/// </summary>
public class BusinessLogicErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ReferenceId { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}

/// <summary>
/// Risposta di errore di sistema
/// </summary>
public class SystemErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ReferenceId { get; set; }
}

/// <summary>
/// Risultato della validazione
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string? ReferenceId { get; set; }
}

/// <summary>
/// Risposta del controllo di salute
/// </summary>
public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
}
