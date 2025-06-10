using Banking.Models.DTOs;

namespace Banking.Logic.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<BankAccountDto>> GetCustomerBankAccountsAsync(int customerId);
        Task<IEnumerable<BankAccountDto>> GetCustomerBankAccountsByTaxCodeAsync(string taxCode);
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDto?> GetCustomerByTaxCodeAsync(string taxCode);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    }
} 