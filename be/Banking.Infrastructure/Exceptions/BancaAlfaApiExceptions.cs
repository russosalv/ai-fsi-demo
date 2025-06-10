namespace Banking.Infrastructure.Exceptions;

/// <summary>
/// Eccezione base per errori dell'API Banca Alfa
/// </summary>
public abstract class BancaAlfaApiException : Exception
{
    public string ErrorCode { get; }
    public DateTime Timestamp { get; }
    public string? ReferenceId { get; }
    public Dictionary<string, object>? Details { get; }

    protected BancaAlfaApiException(
        string errorCode, 
        string message, 
        DateTime timestamp, 
        string? referenceId = null, 
        Dictionary<string, object>? details = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        Timestamp = timestamp;
        ReferenceId = referenceId;
        Details = details;
    }
}

/// <summary>
/// Eccezioni per errori di validazione (HTTP 400)
/// </summary>
public class ValidationException : BancaAlfaApiException
{
    public ValidationException(string errorCode, string message, DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, timestamp, referenceId, details) { }
}

/// <summary>
/// Eccezioni per errori di business logic (HTTP 422)
/// </summary>
public class BusinessLogicException : BancaAlfaApiException
{
    public BusinessLogicException(string errorCode, string message, DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, timestamp, referenceId, details) { }
}

/// <summary>
/// Eccezioni per errori di sistema (HTTP 500)
/// </summary>
public class SystemException : BancaAlfaApiException
{
    public SystemException(string errorCode, string message, DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, timestamp, referenceId, details) { }
}

/// <summary>
/// Eccezioni specifiche per errori di validazione
/// </summary>
public static class ValidationErrors
{
    public class InvalidTaxIdException : ValidationException
    {
        public InvalidTaxIdException(DateTime timestamp, string? referenceId = null)
            : base("INVALID_TAX_ID", "Il formato del codice fiscale non rispetta la normativa italiana", timestamp, referenceId) { }
    }

    public class InvalidAmountException : ValidationException
    {
        public InvalidAmountException(DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
            : base("INVALID_AMOUNT", "L'importo deve essere compreso tra 0.01€ e 5000.00€", timestamp, referenceId, details) { }
    }

    public class MissingRequiredFieldException : ValidationException
    {
        public MissingRequiredFieldException(DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
            : base("MISSING_REQUIRED_FIELD", "Uno o più campi obbligatori non sono stati forniti", timestamp, referenceId, details) { }
    }

    public class InvalidCurrencyException : ValidationException
    {
        public InvalidCurrencyException(DateTime timestamp, string? referenceId = null)
            : base("INVALID_CURRENCY", "Attualmente supportata solo EUR", timestamp, referenceId) { }
    }

    public class DescriptionTooLongException : ValidationException
    {
        public DescriptionTooLongException(DateTime timestamp, string? referenceId = null)
            : base("DESCRIPTION_TOO_LONG", "La causale non può superare i 140 caratteri", timestamp, referenceId) { }
    }
}

/// <summary>
/// Eccezioni specifiche per errori di business logic
/// </summary>
public static class BusinessLogicErrors
{
    public class SenderNotFoundException : BusinessLogicException
    {
        public SenderNotFoundException(DateTime timestamp, string? referenceId = null)
            : base("SENDER_NOT_FOUND", "Il codice fiscale del mittente non è associato a nessun conto", timestamp, referenceId) { }
    }

    public class RecipientNotFoundException : BusinessLogicException
    {
        public RecipientNotFoundException(DateTime timestamp, string? referenceId = null)
            : base("RECIPIENT_NOT_FOUND", "Il codice fiscale del destinatario non è associato a nessun conto", timestamp, referenceId) { }
    }

    public class InsufficientFundsException : BusinessLogicException
    {
        public InsufficientFundsException(DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
            : base("INSUFFICIENT_FUNDS", "Il mittente non ha fondi sufficienti per completare la transazione", timestamp, referenceId, details) { }
    }

    public class AccountBlockedException : BusinessLogicException
    {
        public AccountBlockedException(DateTime timestamp, string? referenceId = null)
            : base("ACCOUNT_BLOCKED", "Il conto del mittente o destinatario è temporaneamente bloccato", timestamp, referenceId) { }
    }

    public class SameAccountTransferException : BusinessLogicException
    {
        public SameAccountTransferException(DateTime timestamp, string? referenceId = null)
            : base("SAME_ACCOUNT_TRANSFER", "Non è possibile effettuare trasferimenti verso il proprio conto", timestamp, referenceId) { }
    }

    public class DailyLimitExceededException : BusinessLogicException
    {
        public DailyLimitExceededException(DateTime timestamp, string? referenceId = null, Dictionary<string, object>? details = null)
            : base("DAILY_LIMIT_EXCEEDED", "Superato il limite giornaliero di trasferimenti P2P (10.000€)", timestamp, referenceId, details) { }
    }

    public class DuplicateReferenceException : BusinessLogicException
    {
        public DuplicateReferenceException(DateTime timestamp, string? referenceId = null)
            : base("DUPLICATE_REFERENCE", "Il reference_id è già stato utilizzato nelle ultime 24 ore", timestamp, referenceId) { }
    }
}

/// <summary>
/// Eccezioni specifiche per errori di sistema
/// </summary>
public static class SystemErrors
{
    public class BancaAlfaSystemException : SystemException
    {
        public BancaAlfaSystemException(DateTime timestamp, string? referenceId = null)
            : base("SYSTEM_ERROR", "Errore temporaneo del sistema, riprovare più tardi", timestamp, referenceId) { }
    }

    public class TimeoutException : SystemException
    {
        public TimeoutException(DateTime timestamp, string? referenceId = null)
            : base("TIMEOUT_ERROR", "La transazione ha impiegato troppo tempo, verificare lo stato", timestamp, referenceId) { }
    }
}
