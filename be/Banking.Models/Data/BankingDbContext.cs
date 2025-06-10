using Microsoft.EntityFrameworkCore;
using Banking.Models.Entities;

namespace Banking.Models.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TaxCode).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TaxCode).IsRequired().HasMaxLength(50);
            });

            // BankAccount configuration
            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Iban).IsUnique();
                entity.Property(e => e.Iban).IsRequired().HasMaxLength(34);
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                
                // Relationship configuration with Customer
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data for testing
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "Mario",
                    LastName = "Rossi",
                    TaxCode = "RSSMRA80A01H501Z",
                    Email = "mario.rossi@email.com",
                    Phone = "3331234567",
                    CreatedDate = DateTime.UtcNow.AddYears(-2),
                    IsActive = true
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Giulia",
                    LastName = "Bianchi",
                    TaxCode = "BNCGLI85M15F205W",
                    Email = "giulia.bianchi@email.com",
                    Phone = "3339876543",
                    CreatedDate = DateTime.UtcNow.AddYears(-1),
                    IsActive = true
                }
            );

            // Seed Bank Accounts
            modelBuilder.Entity<BankAccount>().HasData(
                new BankAccount
                {
                    Id = 1,
                    Iban = "IT60X0542811101000000123456",
                    AccountName = "Main Checking Account",
                    AccountType = "CHECKING",
                    Balance = 1500.00m,
                    OpenDate = DateTime.UtcNow.AddYears(-2),
                    LastUpdated = DateTime.UtcNow,
                    IsActive = true,
                    CustomerId = 1
                },
                new BankAccount
                {
                    Id = 2,
                    Iban = "IT60X0542811101000000654321",
                    AccountName = "Savings Account",
                    AccountType = "SAVINGS",
                    Balance = 5000.00m,
                    OpenDate = DateTime.UtcNow.AddYears(-1),
                    LastUpdated = DateTime.UtcNow,
                    IsActive = true,
                    CustomerId = 1
                },
                new BankAccount
                {
                    Id = 3,
                    Iban = "IT60X0542811101000000789012",
                    AccountName = "Checking Account",
                    AccountType = "CHECKING",
                    Balance = 2750.50m,
                    OpenDate = DateTime.UtcNow.AddMonths(-6),
                    LastUpdated = DateTime.UtcNow,
                    IsActive = true,
                    CustomerId = 2
                }
            );
        }
    }
} 