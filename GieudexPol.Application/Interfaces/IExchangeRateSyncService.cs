using GieudexPol.Application.DTOs;

namespace GieudexPol.Application.Interfaces
{
    public interface IExchangeRateSyncService
    {
        Task<NbpSyncResultDto> SyncNbpRatesAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default);
    }
}
