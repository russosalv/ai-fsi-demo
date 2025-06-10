using Banking.Infrastructure.Constants;
using System.Text.RegularExpressions;

namespace Banking.Infrastructure.Utilities;

/// <summary>
/// Utilità per validazioni varie
/// </summary>
public static class ValidationUtilities
{
    private static readonly Regex TaxIdRegex = new(
        @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
        RegexOptions.Compiled);

    private static readonly Regex AlphanumericRegex = new(
        @"^[a-zA-Z0-9_-]+$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valida il formato di un codice fiscale italiano
    /// </summary>
    /// <param name="taxId">Codice fiscale da validare</param>
    /// <returns>True se valido</returns>
    public static bool IsValidTaxId(string? taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            return false;

        if (taxId.Length != BancaAlfaConstants.Limits.TaxIdLength)
            return false;

        return TaxIdRegex.IsMatch(taxId.ToUpper());
    }

    /// <summary>
    /// Valida che un importo sia entro i limiti consentiti
    /// </summary>
    /// <param name="amount">Importo da validare</param>
    /// <returns>True se valido</returns>
    public static bool IsValidAmount(decimal amount)
    {
        return amount >= BancaAlfaConstants.Limits.MinAmount && 
               amount <= BancaAlfaConstants.Limits.MaxAmount;
    }

    /// <summary>
    /// Valida che una valuta sia supportata
    /// </summary>
    /// <param name="currency">Valuta da validare</param>
    /// <returns>True se valida</returns>
    public static bool IsValidCurrency(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return true; // Default è EUR

        return currency.Equals(BancaAlfaConstants.Currencies.Euro, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Valida la lunghezza della descrizione
    /// </summary>
    /// <param name="description">Descrizione da validare</param>
    /// <returns>True se valida</returns>
    public static bool IsValidDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return true; // Opzionale

        return description.Length <= BancaAlfaConstants.Limits.MaxDescriptionLength;
    }

    /// <summary>
    /// Valida il formato e lunghezza del reference ID
    /// </summary>
    /// <param name="referenceId">Reference ID da validare</param>
    /// <returns>True se valido</returns>
    public static bool IsValidReferenceId(string? referenceId)
    {
        if (string.IsNullOrWhiteSpace(referenceId))
            return true; // Opzionale

        if (referenceId.Length > BancaAlfaConstants.Limits.MaxReferenceIdLength)
            return false;

        return AlphanumericRegex.IsMatch(referenceId);
    }

    /// <summary>
    /// Normalizza un codice fiscale (uppercase, trim)
    /// </summary>
    /// <param name="taxId">Codice fiscale da normalizzare</param>
    /// <returns>Codice fiscale normalizzato</returns>
    public static string? NormalizeTaxId(string? taxId)
    {
        return taxId?.Trim().ToUpper();
    }

    /// <summary>
    /// Normalizza una valuta (uppercase, trim)
    /// </summary>
    /// <param name="currency">Valuta da normalizzare</param>
    /// <returns>Valuta normalizzata</returns>
    public static string NormalizeCurrency(string? currency)
    {
        return string.IsNullOrWhiteSpace(currency) 
            ? BancaAlfaConstants.Currencies.Euro 
            : currency.Trim().ToUpper();
    }
}
