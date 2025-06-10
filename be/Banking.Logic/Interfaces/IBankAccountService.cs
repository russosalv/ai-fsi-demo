using Banking.Models.DTOs;

namespace Banking.Logic.Interfaces
{
    public interface IBankAccountService
    {
        Task<BalanceDto?> GetBalanceByIbanAsync(string iban);
        Task<IEnumerable<BalanceDto>> GetBalancesByIbansAsync(IEnumerable<string> ibans);
        Task<BankAccountDto?> GetBankAccountByIbanAsync(string iban);
        Task<IEnumerable<BankAccountDto>> GetAllBankAccountsAsync();
    }
} 