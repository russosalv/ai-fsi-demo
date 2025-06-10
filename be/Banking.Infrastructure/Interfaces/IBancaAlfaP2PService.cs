using Banking.Infrastructure.Models.Requests;
using Banking.Infrastructure.Models.Responses;

namespace Banking.Infrastructure.Interfaces;

/// <summary>
/// Interfaccia per il servizio di comunicazione con l'API P2P Banking di Banca Alfa
/// </summary>
public interface IBancaAlfaP2PService
{
    /// <summary>
    /// Effettua un trasferimento P2P tra due clienti della banca
    /// </summary>
    /// <param name="request">Dati del trasferimento</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Risposta del trasferimento</returns>
    /// <exception cref="ValidationException">Errore di validazione dei dati</exception>
    /// <exception cref="BusinessLogicException">Errore di business logic</exception>
    /// <exception cref="SystemException">Errore di sistema</exception>
    Task<P2PTransferResponse> TransferAsync(P2PTransferRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida una richiesta di trasferimento P2P localmente prima dell'invio
    /// </summary>
    /// <param name="request">Richiesta da validare</param>
    /// <returns>Lista di errori di validazione (vuota se valida)</returns>
    Task<List<string>> ValidateRequestAsync(P2PTransferRequest request);
}
