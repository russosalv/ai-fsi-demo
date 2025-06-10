namespace Banking.Infrastructure.Constants;

/// <summary>
/// Costanti per l'API Banca Alfa
/// </summary>
public static class BancaAlfaConstants
{
    /// <summary>
    /// Limiti operativi
    /// </summary>
    public static class Limits
    {
        public const decimal MinAmount = 0.01m;
        public const decimal MaxAmount = 5000.00m;
        public const decimal DailyLimit = 10000.00m;
        public const int MaxTransactionsPerDay = 50;
        public const int MaxDescriptionLength = 140;
        public const int MaxReferenceIdLength = 50;
        public const int TaxIdLength = 16;
    }

    /// <summary>
    /// Codici di errore
    /// </summary>
    public static class ErrorCodes
    {
        // Validation errors (400)
        public const string InvalidTaxId = "INVALID_TAX_ID";
        public const string InvalidAmount = "INVALID_AMOUNT";
        public const string MissingRequiredField = "MISSING_REQUIRED_FIELD";
        public const string InvalidCurrency = "INVALID_CURRENCY";
        public const string DescriptionTooLong = "DESCRIPTION_TOO_LONG";

        // Business logic errors (422)
        public const string SenderNotFound = "SENDER_NOT_FOUND";
        public const string RecipientNotFound = "RECIPIENT_NOT_FOUND";
        public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
        public const string AccountBlocked = "ACCOUNT_BLOCKED";
        public const string SameAccountTransfer = "SAME_ACCOUNT_TRANSFER";
        public const string DailyLimitExceeded = "DAILY_LIMIT_EXCEEDED";
        public const string DuplicateReference = "DUPLICATE_REFERENCE";

        // System errors (500)
        public const string SystemError = "SYSTEM_ERROR";
        public const string TimeoutError = "TIMEOUT_ERROR";
    }

    /// <summary>
    /// Valute supportate
    /// </summary>
    public static class Currencies
    {
        public const string Euro = "EUR";
    }

    /// <summary>
    /// Status delle risposte
    /// </summary>
    public static class ResponseStatus
    {
        public const string Success = "success";
        public const string Error = "error";
    }
}
