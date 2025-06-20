using Banking.Infrastructure.DTOs.Request;
using Banking.Models.DTOs;

namespace Banking.Infrastructure.Interfaces;

public interface IP2PTransferService
{
    /// <summary>
    /// Esegue un trasferimento P2P utilizzando i modelli del dominio
    /// </summary>
    /// <param name="senderTaxId">Codice fiscale del mittente</param>
    /// <param name="recipientTaxId">Codice fiscale del destinatario</param>
    /// <param name="amount">Importo da trasferire</param>
    /// <param name="description">Descrizione opzionale del trasferimento</param>
    /// <param name="referenceId">ID di riferimento opzionale</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Dettagli del trasferimento completato</returns>
    Task<P2PTransferResult> ExecuteTransferAsync(
        string senderTaxId, 
        string recipientTaxId, 
        decimal amount, 
        string? description = null, 
        string? referenceId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida i parametri di input per un trasferimento P2P
    /// </summary>
    /// <param name="request">Richiesta da validare</param>
    /// <returns>Lista degli errori di validazione</returns>
    Task<List<string>> ValidateTransferRequestAsync(P2PTransferRequest request);
}
