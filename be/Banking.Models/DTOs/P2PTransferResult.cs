namespace Banking.Models.DTOs;

public class P2PTransferResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string SenderTaxId { get; set; } = string.Empty;
    public string SenderIban { get; set; } = string.Empty;
    public string RecipientTaxId { get; set; } = string.Empty;
    public string RecipientIban { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string? ReferenceId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? ErrorDetails { get; set; }
}
