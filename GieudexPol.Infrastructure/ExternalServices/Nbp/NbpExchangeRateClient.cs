using GieudexPol.Application.DTOs;
using GieudexPol.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace GieudexPol.Infrastructure.ExternalServices.Nbp
{
    public class NbpExchangeRateClient : INbpExchangeRateClient
    {
        private readonly HttpClient _httpClient;

        public NbpExchangeRateClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<NbpExchangeRateTableDto>> GetTableCRatesAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"exchangerates/tables/C/{from:yyyy-MM-dd}/{to:yyyy-MM-dd}/?format=json";
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Array.Empty<NbpExchangeRateTableDto>();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var message = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"NBP rejected range {from:yyyy-MM-dd} - {to:yyyy-MM-dd}: {message}");
            }

            response.EnsureSuccessStatusCode();

            var tables = await response.Content.ReadFromJsonAsync<List<NbpExchangeRateTableDto>>(
                cancellationToken: cancellationToken);

            return tables ?? new List<NbpExchangeRateTableDto>();
        }

        public async Task<IReadOnlyList<NbpExchangeRateTableDto>> GetLatestExchangeRatesAsync(
            CancellationToken cancellationToken = default)
        {
            var endpoint = "exchangerates/tables/A/?format=json";
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Array.Empty<NbpExchangeRateTableDto>();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var message = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"NBP API request failed: {message}");
            }

            response.EnsureSuccessStatusCode();

            var tables = await response.Content.ReadFromJsonAsync<List<NbpExchangeRateTableDto>>(
                cancellationToken: cancellationToken);

            return tables ?? new List<NbpExchangeRateTableDto>();
        }
    }
}