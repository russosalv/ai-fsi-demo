using Microsoft.AspNetCore.Mvc;
using Banking.Logic.Interfaces;
using Banking.Models.DTOs;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets all bank accounts for a specific customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>List of bank accounts for the customer</returns>
        [HttpGet("{id}/bank-accounts")]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetCustomerBankAccounts(int id)
        {
            try
            {
                // First verify the customer exists
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found");
                }

                var bankAccounts = await _customerService.GetCustomerBankAccountsAsync(id);
                return Ok(bankAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank accounts for customer {CustomerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets all bank accounts for a customer by Tax Code (for login authentication)
        /// </summary>
        /// <param name="taxCode">Customer Tax Code</param>
        /// <returns>List of bank accounts for the customer</returns>
        [HttpGet("taxcode/{taxCode}/bank-accounts")]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetCustomerBankAccountsByTaxCode(string taxCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(taxCode))
                {
                    return BadRequest("Tax Code cannot be empty");
                }

                var bankAccounts = await _customerService.GetCustomerBankAccountsByTaxCodeAsync(taxCode);
                if (!bankAccounts.Any())
                {
                    return NotFound($"No customer found with Tax Code {taxCode}");
                }

                return Ok(bankAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank accounts for customer with tax code {TaxCode}", taxCode);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 