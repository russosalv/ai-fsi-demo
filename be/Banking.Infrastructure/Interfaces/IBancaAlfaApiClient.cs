using Banking.Infrastructure.DTOs.Request;
using Banking.Infrastructure.DTOs.Response;

namespace Banking.Infrastructure.Interfaces;

public interface IBancaAlfaApiClient
{
    /// <summary>
    /// Effettua un trasferimento P2P tramite l'API di Banca Alfa
    /// </summary>
    /// <param name="request">Richiesta di trasferimento P2P</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Risposta del trasferimento P2P</returns>
    /// <exception cref="ValidationException">Errori di validazione (HTTP 400)</exception>
    /// <exception cref="BusinessLogicException">Errori di business logic (HTTP 422)</exception>
    /// <exception cref="SystemException">Errori di sistema (HTTP 500)</exception>
    Task<P2PTransferResponse> ExecuteP2PTransferAsync(P2PTransferRequest request, CancellationToken cancellationToken = default);
}
