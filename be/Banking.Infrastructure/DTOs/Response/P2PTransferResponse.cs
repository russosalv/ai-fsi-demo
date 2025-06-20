using System.Text.Json.Serialization;

namespace Banking.Infrastructure.DTOs.Response;

public class P2PTransferResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("sender")]
    public ParticipantInfo Sender { get; set; } = new();

    [JsonPropertyName("recipient")]
    public ParticipantInfo Recipient { get; set; } = new();

    [JsonPropertyName("fees")]
    public FeeInfo Fees { get; set; } = new();

    [JsonPropertyName("execution_date")]
    public DateTime ExecutionDate { get; set; }

    [JsonPropertyName("reference_id")]
    public string? ReferenceId { get; set; }
}

public class ParticipantInfo
{
    [JsonPropertyName("tax_id")]
    public string TaxId { get; set; } = string.Empty;

    [JsonPropertyName("account_iban")]
    public string AccountIban { get; set; } = string.Empty;
}

public class FeeInfo
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}
