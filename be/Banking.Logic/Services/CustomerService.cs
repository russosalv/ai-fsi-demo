using Banking.Logic.Interfaces;
using Banking.Models.Data;
using Banking.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Banking.Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankingDbContext _context;

        public CustomerService(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankAccountDto>> GetCustomerBankAccountsAsync(int customerId)
        {
            var bankAccounts = await _context.BankAccounts
                .Include(ba => ba.Customer)
                .Where(ba => ba.CustomerId == customerId && ba.IsActive)
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

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.Id == customerId && c.IsActive)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    TaxCode = c.TaxCode,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedDate = c.CreatedDate,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            return customer;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    TaxCode = c.TaxCode,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedDate = c.CreatedDate,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return customers;
        }

        public async Task<CustomerDto?> GetCustomerByTaxCodeAsync(string taxCode)
        {
            var customer = await _context.Customers
                .Where(c => c.TaxCode == taxCode && c.IsActive)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    TaxCode = c.TaxCode,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedDate = c.CreatedDate,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            return customer;
        }

        public async Task<IEnumerable<BankAccountDto>> GetCustomerBankAccountsByTaxCodeAsync(string taxCode)
        {
            var bankAccounts = await _context.BankAccounts
                .Include(ba => ba.Customer)
                .Where(ba => ba.Customer.TaxCode == taxCode && ba.Customer.IsActive && ba.IsActive)
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