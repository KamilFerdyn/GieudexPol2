using GieudexPol.Application.DTOs;
using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GieudexPol.Infrastructure.Services
{
    public class ExchangeRateSyncService : IExchangeRateSyncService
    {
        private const int MaxNbpRangeDays = 93;
        private const string NbpSourceCode = "NBP";
        private const string NbpSourceName = "Narodowy Bank Polski";

        private readonly ApplicationDbContext _context;
        private readonly INbpExchangeRateClient _nbpClient;
        private readonly ILogger<ExchangeRateSyncService> _logger;

        public ExchangeRateSyncService(
            ApplicationDbContext context,
            INbpExchangeRateClient nbpClient,
            ILogger<ExchangeRateSyncService> logger)
        {
            _context = context;
            _nbpClient = nbpClient;
            _logger = logger;
        }

        public async Task<NbpSyncResultDto> SyncNbpRatesAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default)
        {
            from = from.Date;
            to = to.Date;

            if (from > to)
            {
                throw new ArgumentException("From date cannot be later than to date.");
            }

            var today = DateTime.Today;
            if (to > today)
            {
                throw new ArgumentException("To date cannot be later than today.");
            }

            var result = new NbpSyncResultDto
            {
                From = from,
                To = to
            };

            _logger.LogInformation("Starting NBP exchange rate sync from {From} to {To}.", from, to);

            var rateSource = await GetOrCreateNbpRateSourceAsync(cancellationToken);
            var currencies = await _context.Currencies.ToDictionaryAsync(
                currency => currency.Symbol,
                cancellationToken);
            var existingRates = await LoadExistingRateKeysAsync(rateSource.Id, from, to, cancellationToken);

            foreach (var (rangeFrom, rangeTo) in SplitIntoNbpRanges(from, to))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rangeLabel = $"{rangeFrom:yyyy-MM-dd} - {rangeTo:yyyy-MM-dd}";
                result.ProcessedRanges.Add(rangeLabel);
                _logger.LogInformation("Fetching NBP table C range {Range}.", rangeLabel);

                IReadOnlyList<NbpExchangeRateTableDto> tables;
                try
                {
                    tables = await _nbpClient.GetTableCRatesAsync(rangeFrom, rangeTo, cancellationToken);
                }
                catch (Exception ex) when (ex is InvalidOperationException or HttpRequestException)
                {
                    result.Warnings.Add($"Range {rangeLabel}: {ex.Message}");
                    _logger.LogWarning(ex, "NBP range {Range} failed.", rangeLabel);
                    continue;
                }

                result.TablesFetched += tables.Count;

                if (tables.Count == 0)
                {
                    result.Warnings.Add($"Range {rangeLabel}: NBP returned no table C data.");
                    continue;
                }

                foreach (var table in tables)
                {
                    var effectiveDate = table.EffectiveDate.Date;

                    foreach (var rate in table.Rates)
                    {
                        var currencyCode = rate.Code.Trim().ToUpperInvariant();

                        if (!currencies.TryGetValue(currencyCode, out var currency))
                        {
                            currency = new Currency
                            {
                                Symbol = currencyCode,
                                Name = rate.Currency,
                                IsActive = true
                            };

                            currencies[currencyCode] = currency;
                            await _context.Currencies.AddAsync(currency, cancellationToken);
                        }

                        var existingKey = new ExistingRateKey(currencyCode, effectiveDate);
                        if (existingRates.Contains(existingKey))
                        {
                            result.Skipped++;
                            continue;
                        }

                        await _context.ExchangeRates.AddAsync(new ExchangeRate
                        {
                            Currency = currency,
                            RateSource = rateSource,
                            BuyPrice = rate.Bid,
                            SellPrice = rate.Ask,
                            EffectiveDate = effectiveDate,
                            FetchedAt = DateTime.UtcNow
                        }, cancellationToken);

                        existingRates.Add(existingKey);
                        result.Added++;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Finished NBP exchange rate sync. Added {Added}, skipped {Skipped}, tables fetched {TablesFetched}.",
                result.Added,
                result.Skipped,
                result.TablesFetched);

            return result;
        }

        private async Task<RateSource> GetOrCreateNbpRateSourceAsync(CancellationToken cancellationToken)
        {
            var rateSource = await _context.RateSources
                .FirstOrDefaultAsync(source => source.Code == NbpSourceCode, cancellationToken);

            if (rateSource != null)
            {
                rateSource.Name = NbpSourceName;
                rateSource.IsActive = true;
                return rateSource;
            }

            rateSource = new RateSource
            {
                Code = NbpSourceCode,
                Name = NbpSourceName,
                IsActive = true
            };

            await _context.RateSources.AddAsync(rateSource, cancellationToken);
            return rateSource;
        }

        private async Task<HashSet<ExistingRateKey>> LoadExistingRateKeysAsync(
            int rateSourceId,
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken)
        {
            if (rateSourceId == 0)
            {
                return new HashSet<ExistingRateKey>();
            }

            var keys = await _context.ExchangeRates
                .AsNoTracking()
                .Where(rate =>
                    rate.RateSourceId == rateSourceId &&
                    rate.EffectiveDate >= from &&
                    rate.EffectiveDate <= to)
                .Select(rate => new ExistingRateKey(rate.Currency.Symbol, rate.EffectiveDate.Date))
                .ToListAsync(cancellationToken);

            return keys.ToHashSet();
        }

        private static IEnumerable<(DateTime From, DateTime To)> SplitIntoNbpRanges(DateTime from, DateTime to)
        {
            var rangeFrom = from.Date;

            while (rangeFrom <= to.Date)
            {
                var rangeTo = rangeFrom.AddDays(MaxNbpRangeDays - 1);
                if (rangeTo > to.Date)
                {
                    rangeTo = to.Date;
                }

                yield return (rangeFrom, rangeTo);
                rangeFrom = rangeTo.AddDays(1);
            }
        }

        private sealed record ExistingRateKey(string CurrencyCode, DateTime EffectiveDate);
    }
}
