using Microsoft.AspNetCore.Mvc;
using Banking.Logic.Interfaces;
using Banking.Models.DTOs;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly ILogger<BankAccountsController> _logger;

        public BankAccountsController(IBankAccountService bankAccountService, ILogger<BankAccountsController> logger)
        {
            _bankAccountService = bankAccountService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all bank accounts
        /// </summary>
        /// <returns>List of bank accounts</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetBankAccounts()
        {
            try
            {
                var bankAccounts = await _bankAccountService.GetAllBankAccountsAsync();
                return Ok(bankAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank accounts");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a bank account by IBAN
        /// </summary>
        /// <param name="iban">Bank account IBAN</param>
        /// <returns>Bank account details</returns>
        [HttpGet("iban/{iban}")]
        public async Task<ActionResult<BankAccountDto>> GetBankAccountByIban(string iban)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(iban))
                {
                    return BadRequest("IBAN cannot be empty");
                }

                var bankAccount = await _bankAccountService.GetBankAccountByIbanAsync(iban);
                if (bankAccount == null)
                {
                    return NotFound($"Bank account with IBAN {iban} not found");
                }
                return Ok(bankAccount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank account {Iban}", iban);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets the balance for a specific IBAN
        /// </summary>
        /// <param name="iban">Bank account IBAN</param>
        /// <returns>Balance information</returns>
        [HttpGet("iban/{iban}/balance")]
        public async Task<ActionResult<BalanceDto>> GetBalanceByIban(string iban)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(iban))
                {
                    return BadRequest("IBAN cannot be empty");
                }

                var balance = await _bankAccountService.GetBalanceByIbanAsync(iban);
                if (balance == null)
                {
                    return NotFound($"Bank account with IBAN {iban} not found");
                }
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance for IBAN {Iban}", iban);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets balances for multiple IBANs
        /// </summary>
        /// <param name="ibans">List of IBANs (comma-separated)</param>
        /// <returns>List of balance information for each IBAN</returns>
        [HttpGet("balances")]
        public async Task<ActionResult<IEnumerable<BalanceDto>>> GetBalancesByIbans([FromQuery] string ibans)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ibans))
                {
                    return BadRequest("IBANs parameter cannot be empty");
                }

                var ibanList = ibans.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(i => i.Trim())
                                   .Where(i => !string.IsNullOrWhiteSpace(i))
                                   .ToList();

                if (!ibanList.Any())
                {
                    return BadRequest("No valid IBANs provided");
                }

                var balances = await _bankAccountService.GetBalancesByIbansAsync(ibanList);
                return Ok(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balances for IBANs {Ibans}", ibans);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 