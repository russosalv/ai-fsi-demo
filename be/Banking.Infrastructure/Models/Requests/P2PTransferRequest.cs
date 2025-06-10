using System.Text.Json.Serialization;

namespace Banking.Infrastructure.Models.Requests;

public class P2PTransferRequest
{
    [JsonPropertyName("sender_tax_id")]
    public string SenderTaxId { get; set; } = string.Empty;

    [JsonPropertyName("recipient_tax_id")]
    public string RecipientTaxId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "EUR";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("reference_id")]
    public string? ReferenceId { get; set; }
}
