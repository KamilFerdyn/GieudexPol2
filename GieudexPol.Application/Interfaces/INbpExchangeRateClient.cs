using GieudexPol.Application.DTOs;

namespace GieudexPol.Application.Interfaces
{
    public interface INbpExchangeRateClient
    {
        Task<IReadOnlyList<NbpExchangeRateTableDto>> GetTableCRatesAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<NbpExchangeRateTableDto>> GetLatestExchangeRatesAsync(
            CancellationToken cancellationToken = default);
    }
}