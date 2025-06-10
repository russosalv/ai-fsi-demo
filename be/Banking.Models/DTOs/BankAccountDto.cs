namespace Banking.Models.DTOs
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string Iban { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
} 