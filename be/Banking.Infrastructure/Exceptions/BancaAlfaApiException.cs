namespace Banking.Infrastructure.Exceptions;

public class BancaAlfaApiException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }
    public Dictionary<string, object>? Details { get; }
    public string? ReferenceId { get; }

    public BancaAlfaApiException(
        string errorCode, 
        string message, 
        int statusCode,
        string? referenceId = null,
        Dictionary<string, object>? details = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        ReferenceId = referenceId;
        Details = details;
    }

    public BancaAlfaApiException(
        string errorCode, 
        string message, 
        int statusCode,
        Exception innerException,
        string? referenceId = null,
        Dictionary<string, object>? details = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        ReferenceId = referenceId;
        Details = details;
    }
}

public class ValidationException : BancaAlfaApiException
{
    public ValidationException(string errorCode, string message, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, 400, referenceId, details)
    {
    }
}

public class BusinessLogicException : BancaAlfaApiException
{
    public BusinessLogicException(string errorCode, string message, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, 422, referenceId, details)
    {
    }
}

public class BancaAlfaSystemException : BancaAlfaApiException
{
    public BancaAlfaSystemException(string errorCode, string message, string? referenceId = null, Dictionary<string, object>? details = null)
        : base(errorCode, message, 500, referenceId, details)
    {
    }
}
