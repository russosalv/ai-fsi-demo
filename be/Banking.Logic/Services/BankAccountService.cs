using Banking.Logic.Interfaces;
using Banking.Models.Data;
using Banking.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Banking.Logic.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly BankingDbContext _context;

        public BankAccountService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<BalanceDto?> GetBalanceByIbanAsync(string iban)
        {
            var bankAccount = await _context.BankAccounts
                .Where(ba => ba.Iban == iban && ba.IsActive)
                .Select(ba => new BalanceDto
                {
                    Iban = ba.Iban,
                    AccountName = ba.AccountName,
                    AccountType = ba.AccountType,
                    Balance = ba.Balance,
                    LastUpdated = ba.LastUpdated,
                    IsActive = ba.IsActive,
                    Currency = "EUR"
                })
                .FirstOrDefaultAsync();

            return bankAccount;
        }

        public async Task<IEnumerable<BalanceDto>> GetBalancesByIbansAsync(IEnumerable<string> ibans)
        {
            var bankAccounts = await _context.BankAccounts
                .Where(ba => ibans.Contains(ba.Iban) && ba.IsActive)
                .Select(ba => new BalanceDto
                {
                    Iban = ba.Iban,
                    AccountName = ba.AccountName,
                    AccountType = ba.AccountType,
                    Balance = ba.Balance,
                    LastUpdated = ba.LastUpdated,
                    IsActive = ba.IsActive,
                    Currency = "EUR"
                })
                .ToListAsync();

            return bankAccounts;
        }

        public async Task<BankAccountDto?> GetBankAccountByIbanAsync(string iban)
        {
            var bankAccount = await _context.BankAccounts
                .Include(ba => ba.Customer)
                .Where(ba => ba.Iban == iban && ba.IsActive)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    Iban = ba.Iban,
                    AccountName = ba.AccountName,
                    AccountType = ba.AccountType,
                    Balance = ba.Balance,
                    OpenDate = ba.OpenDate,
                    LastUpdated = ba.LastUpdated,
                    IsActive = ba.IsActive,
                    CustomerId = ba.CustomerId,
                    CustomerName = $"{ba.Customer.FirstName} {ba.Customer.LastName}"
                })
                .FirstOrDefaultAsync();

            return bankAccount;
        }

        public async Task<IEnumerable<BankAccountDto>> GetAllBankAccountsAsync()
        {
            var bankAccounts = await _context.BankAccounts
                .Include(ba => ba.Customer)
                .Where(ba => ba.IsActive)
                .Select(ba => new BankAccountDto
                {
                    Id = ba.Id,
                    Iban = ba.Iban,
                    AccountName = ba.AccountName,
                    AccountType = ba.AccountType,
                    Balance = ba.Balance,
                    OpenDate = ba.OpenDate,
                    LastUpdated = ba.LastUpdated,
                    IsActive = ba.IsActive,
                    CustomerId = ba.CustomerId,
                    CustomerName = $"{ba.Customer.FirstName} {ba.Customer.LastName}"
                })
                .ToListAsync();

            return bankAccounts;
        }
    }
} 