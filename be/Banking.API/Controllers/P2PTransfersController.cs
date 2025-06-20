using Banking.Infrastructure.Interfaces;
using Banking.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class P2PTransfersController : ControllerBase
{
    private readonly IP2PTransferService _p2pTransferService;
    private readonly ILogger<P2PTransfersController> _logger;

    public P2PTransfersController(IP2PTransferService p2pTransferService, ILogger<P2PTransfersController> logger)
    {
        _p2pTransferService = p2pTransferService;
        _logger = logger;
    }

    /// <summary>
    /// Esegue un trasferimento P2P tra due clienti tramite l'API di Banca Alfa
    /// </summary>
    /// <param name="request">Dati del trasferimento</param>
    /// <returns>Risultato del trasferimento</returns>
    [HttpPost]
    [ProducesResponseType(typeof(P2PTransferResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExecuteTransfer([FromBody] P2PTransferRequestDto request)
    {
        try
        {
            _logger.LogInformation("Received P2P transfer request from {SenderTaxId} to {RecipientTaxId}", 
                request.SenderTaxId, request.RecipientTaxId);

            var result = await _p2pTransferService.ExecuteTransferAsync(
                request.SenderTaxId,
                request.RecipientTaxId,
                request.Amount,
                request.Description,
                request.ReferenceId);

            if (result.IsSuccess)
            {
                _logger.LogInformation("P2P transfer completed successfully. Transaction ID: {TransactionId}", 
                    result.TransactionId);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("P2P transfer failed. Error: {ErrorCode} - {ErrorMessage}", 
                    result.ErrorCode, result.ErrorMessage);
                
                return result.ErrorCode switch
                {
                    "VALIDATION_ERROR" => BadRequest(result),
                    "INVALID_TAX_ID" => BadRequest(result),
                    "INVALID_AMOUNT" => BadRequest(result),
                    "MISSING_REQUIRED_FIELD" => BadRequest(result),
                    "INVALID_CURRENCY" => BadRequest(result),
                    "DESCRIPTION_TOO_LONG" => BadRequest(result),
                    "SENDER_NOT_FOUND" => UnprocessableEntity(result),
                    "RECIPIENT_NOT_FOUND" => UnprocessableEntity(result),
                    "INSUFFICIENT_FUNDS" => UnprocessableEntity(result),
                    "ACCOUNT_BLOCKED" => UnprocessableEntity(result),
                    "SAME_ACCOUNT_TRANSFER" => UnprocessableEntity(result),
                    "DAILY_LIMIT_EXCEEDED" => UnprocessableEntity(result),
                    "DUPLICATE_REFERENCE" => UnprocessableEntity(result),
                    _ => StatusCode(500, result)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during P2P transfer");
            return StatusCode(500, "Errore interno del server");
        }
    }

    /// <summary>
    /// Ottiene i limiti operativi per i trasferimenti P2P
    /// </summary>
    /// <returns>Limiti operativi</returns>
    [HttpGet("limits")]
    [ProducesResponseType(typeof(P2PTransferLimitsDto), StatusCodes.Status200OK)]
    public IActionResult GetTransferLimits()
    {
        var limits = new P2PTransferLimitsDto
        {
            MinAmount = 0.01m,
            MaxAmount = 5000.00m,
            DailyLimit = 10000.00m,
            MaxDailyTransactions = 50,
            MaxDescriptionLength = 140,
            MaxReferenceIdLength = 50,
            SupportedCurrencies = new[] { "EUR" }
        };

        return Ok(limits);
    }
}

/// <summary>
/// DTO per la richiesta di trasferimento P2P
/// </summary>
public class P2PTransferRequestDto
{
    public string SenderTaxId { get; set; } = string.Empty;
    public string RecipientTaxId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ReferenceId { get; set; }
}

/// <summary>
/// DTO per i limiti operativi dei trasferimenti P2P
/// </summary>
public class P2PTransferLimitsDto
{
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal DailyLimit { get; set; }
    public int MaxDailyTransactions { get; set; }
    public int MaxDescriptionLength { get; set; }
    public int MaxReferenceIdLength { get; set; }
    public string[] SupportedCurrencies { get; set; } = Array.Empty<string>();
}
