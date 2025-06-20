namespace Banking.Infrastructure.Constants;

public static class BancaAlfaErrorCodes
{
    // Errori di validazione (HTTP 400)
    public const string InvalidTaxId = "INVALID_TAX_ID";
    public const string InvalidAmount = "INVALID_AMOUNT";
    public const string MissingRequiredField = "MISSING_REQUIRED_FIELD";
    public const string InvalidCurrency = "INVALID_CURRENCY";
    public const string DescriptionTooLong = "DESCRIPTION_TOO_LONG";

    // Errori di business logic (HTTP 422)
    public const string SenderNotFound = "SENDER_NOT_FOUND";
    public const string RecipientNotFound = "RECIPIENT_NOT_FOUND";
    public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
    public const string AccountBlocked = "ACCOUNT_BLOCKED";
    public const string SameAccountTransfer = "SAME_ACCOUNT_TRANSFER";
    public const string DailyLimitExceeded = "DAILY_LIMIT_EXCEEDED";
    public const string DuplicateReference = "DUPLICATE_REFERENCE";

    // Errori di sistema (HTTP 500)
    public const string SystemError = "SYSTEM_ERROR";
    public const string TimeoutError = "TIMEOUT_ERROR";

    // Errori aggiuntivi per la gestione interna
    public const string NetworkError = "NETWORK_ERROR";
    public const string DeserializationError = "DESERIALIZATION_ERROR";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string UnknownError = "UNKNOWN_ERROR";
}

public static class BancaAlfaLimits
{
    public const decimal MinAmount = 0.01m;
    public const decimal MaxAmount = 5000.00m;
    public const decimal DailyLimit = 10000.00m;
    public const int MaxDailyTransactions = 50;
    public const int MaxDescriptionLength = 140;
    public const int MaxReferenceIdLength = 50;
    public const int TaxIdLength = 16;
    public const string SupportedCurrency = "EUR";
}
