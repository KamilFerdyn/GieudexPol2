using GieudexPol.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GieudexPol.Infrastructure.Data
{
    public static class DevelopmentDataSeeder
    {
        private const string DevelopmentSourceCode = "MOCK_BANK_A";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(DevelopmentDataSeeder));

            if (!await context.Database.CanConnectAsync())
            {
                logger.LogWarning("Development seed skipped because the database is not available.");
                return;
            }

            var addedCurrencies = await SeedCurrenciesAsync(context);
            var rateSource = await SeedRateSourceAsync(context);
            var addedRates = await SeedExchangeRatesAsync(context, rateSource);

            logger.LogInformation(
                "Development seed completed. Added {CurrencyCount} currencies and {RateCount} exchange rates.",
                addedCurrencies,
                addedRates);
        }

        private static async Task<int> SeedCurrenciesAsync(ApplicationDbContext context)
        {
            var seedCurrencies = new[]
            {
                new Currency { Symbol = "EUR", Name = "Euro", IsActive = true },
                new Currency { Symbol = "USD", Name = "US Dollar", IsActive = true },
                new Currency { Symbol = "CHF", Name = "Swiss Franc", IsActive = true },
                new Currency { Symbol = "GBP", Name = "British Pound", IsActive = true }
            };

            var existingSymbols = await context.Currencies
                .Where(currency => seedCurrencies.Select(seed => seed.Symbol).Contains(currency.Symbol))
                .Select(currency => currency.Symbol)
                .ToListAsync();

            var currenciesToAdd = seedCurrencies
                .Where(currency => !existingSymbols.Contains(currency.Symbol))
                .ToList();

            if (currenciesToAdd.Count == 0)
            {
                return 0;
            }

            await context.Currencies.AddRangeAsync(currenciesToAdd);
            await context.SaveChangesAsync();

            return currenciesToAdd.Count;
        }

        private static async Task<RateSource> SeedRateSourceAsync(ApplicationDbContext context)
        {
            var rateSource = await context.RateSources
                .FirstOrDefaultAsync(source => source.Code == DevelopmentSourceCode);

            if (rateSource != null)
            {
                return rateSource;
            }

            rateSource = new RateSource
            {
                Code = DevelopmentSourceCode,
                Name = "Development Mock Bank A",
                IsActive = true
            };

            await context.RateSources.AddAsync(rateSource);
            await context.SaveChangesAsync();

            return rateSource;
        }

        private static async Task<int> SeedExchangeRatesAsync(ApplicationDbContext context, RateSource rateSource)
        {
            var startDate = new DateTime(2026, 1, 1);
            var endDate = DateTime.Today;

            if (endDate < startDate)
            {
                return 0;
            }

            var currencyModels = new[]
            {
                new CurrencyRateSeed("EUR", 4.30m, 0.00035m, 0.045m),
                new CurrencyRateSeed("USD", 3.95m, -0.00015m, 0.040m),
                new CurrencyRateSeed("CHF", 4.55m, 0.00025m, 0.055m),
                new CurrencyRateSeed("GBP", 5.05m, 0.00020m, 0.065m)
            };

            var symbols = currencyModels.Select(model => model.Symbol).ToList();
            var currencies = await context.Currencies
                .Where(currency => symbols.Contains(currency.Symbol))
                .ToDictionaryAsync(currency => currency.Symbol);

            var existingRateKeys = await context.ExchangeRates
                .Where(rate =>
                    rate.RateSourceId == rateSource.Id &&
                    rate.EffectiveDate >= startDate &&
                    rate.EffectiveDate <= endDate)
                .Select(rate => new { rate.CurrencyId, rate.EffectiveDate })
                .ToListAsync();

            var existingRates = existingRateKeys
                .Select(rate => (rate.CurrencyId, Date: rate.EffectiveDate.Date))
                .ToHashSet();

            var random = new Random(12345);
            var ratesToAdd = new List<ExchangeRate>();

            foreach (var currencyModel in currencyModels)
            {
                if (!currencies.TryGetValue(currencyModel.Symbol, out var currency))
                {
                    continue;
                }

                var midPrice = currencyModel.StartMidPrice;
                var businessDayIndex = 0;

                foreach (var date in EachBusinessDay(startDate, endDate))
                {
                    var wave = (decimal)Math.Sin(businessDayIndex / 12.0) * 0.012m;
                    var randomMove = ((decimal)random.NextDouble() - 0.5m) * 0.018m;

                    midPrice += currencyModel.DailyTrend + wave / 30m + randomMove;
                    midPrice = Math.Max(midPrice, currencyModel.StartMidPrice - 0.25m);
                    midPrice = Math.Min(midPrice, currencyModel.StartMidPrice + 0.25m);

                    var spreadJitter = ((decimal)random.NextDouble() - 0.5m) * 0.010m;
                    var spread = Math.Max(0.030m, currencyModel.BaseSpread + spreadJitter);
                    var buyPrice = Math.Round(midPrice - spread / 2m, 4);
                    var sellPrice = Math.Round(midPrice + spread / 2m, 4);

                    if (!existingRates.Contains((currency.Id, date.Date)))
                    {
                        ratesToAdd.Add(new ExchangeRate
                        {
                            CurrencyId = currency.Id,
                            RateSourceId = rateSource.Id,
                            EffectiveDate = date.Date,
                            FetchedAt = date.Date.AddHours(16),
                            BuyPrice = buyPrice,
                            SellPrice = sellPrice
                        });
                    }

                    businessDayIndex++;
                }
            }

            if (ratesToAdd.Count == 0)
            {
                return 0;
            }

            await context.ExchangeRates.AddRangeAsync(ratesToAdd);
            await context.SaveChangesAsync();

            return ratesToAdd.Count;
        }

        private static IEnumerable<DateTime> EachBusinessDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
                {
                    yield return date;
                }
            }
        }

        private sealed record CurrencyRateSeed(
            string Symbol,
            decimal StartMidPrice,
            decimal DailyTrend,
            decimal BaseSpread);
    }
}
