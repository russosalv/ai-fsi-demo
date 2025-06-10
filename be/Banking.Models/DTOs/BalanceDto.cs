namespace Banking.Models.DTOs
{
    public class BalanceDto
    {
        public string Iban { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public string Currency { get; set; } = "EUR";
    }
} 