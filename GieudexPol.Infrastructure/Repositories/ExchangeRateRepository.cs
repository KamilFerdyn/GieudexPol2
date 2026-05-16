using GieudexPol.Application.Interfaces;
using GieudexPol.Application.DTOs;
using GieudexPol.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GieudexPol.Infrastructure.Repositories
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ApplicationDbContext context, INbpExchangeRateClient nbpExchangeRateClient) : base(context)
        {
            _nbpExchangeRateClient = nbpExchangeRateClient;
        }

        private readonly INbpExchangeRateClient _nbpExchangeRateClient;

        public async Task<ExchangeRate> GetByCurrencyPairAsync(string baseCurrencySymbol, string targetCurrencySymbol)
        {
            if (targetCurrencySymbol != "PLN")
            {
                return null;
            }

            return await _dbSet
                .Include(er => er.Currency)
                .Include(er => er.RateSource)
                .Where(er => er.Currency.Symbol == baseCurrencySymbol)
                .OrderByDescending(er => er.EffectiveDate)
                .ThenByDescending(er => er.FetchedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExchangeRateChartPointDto>> GetChartDataAsync(
            string currencyCode,
            string sourceCode,
            DateTime from,
            DateTime to)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(er =>
                    er.Currency.Symbol == currencyCode &&
                    er.RateSource.Code == sourceCode &&
                    er.EffectiveDate >= from.Date &&
                    er.EffectiveDate <= to.Date)
                .OrderBy(er => er.EffectiveDate)
                .Select(er => new ExchangeRateChartPointDto
                {
                    Date = er.EffectiveDate,
                    BuyPrice = er.BuyPrice,
                    SellPrice = er.SellPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ExchangeRateChartPointDto>> GetRatesForChartAsync(
            string currencySymbol,
            string sourceCode,
            DateTime from,
            DateTime to)
        {
            return await GetChartDataAsync(currencySymbol, sourceCode, from, to);
        }

        public async Task<IEnumerable<ExchangeRateTableRowDto>> GetLatestRatesAsync(string sourceCode)
        {
            var latestRatesQuery = _dbSet
                .AsNoTracking()
                .Where(er => er.RateSource.Code == sourceCode)
                .GroupBy(er => er.CurrencyId)
                .Select(group => new
                {
                    CurrencyId = group.Key,
                    EffectiveDate = group.Max(er => er.EffectiveDate)
                });

            return await _dbSet
                .AsNoTracking()
                .Join(
                    latestRatesQuery,
                    rate => new { rate.CurrencyId, rate.EffectiveDate },
                    latest => new { latest.CurrencyId, latest.EffectiveDate },
                    (rate, latest) => rate)
                .Where(er => er.RateSource.Code == sourceCode)
                .OrderBy(er => er.Currency.Symbol)
                .Select(er => new ExchangeRateTableRowDto
                {
                    CurrencyCode = er.Currency.Symbol,
                    CurrencyName = er.Currency.Name,
                    SourceCode = er.RateSource.Code,
                    SourceName = er.RateSource.Name,
                    EffectiveDate = er.EffectiveDate,
                    BuyPrice = er.BuyPrice,
                    SellPrice = er.SellPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ExchangeRateTableRowDto>> GetLatestRatesFromNbpAsync(CancellationToken cancellationToken = default)
        {
            var nbpRates = await _nbpExchangeRateClient.GetLatestExchangeRatesAsync(cancellationToken);

            var latestRates = new List<ExchangeRateTableRowDto>();

            foreach (var table in nbpRates)
            {
                foreach (var rate in table.Rates)
                {
                    latestRates.Add(new ExchangeRateTableRowDto
                    {
                        CurrencyCode = rate.Currency,
                        CurrencyName = rate.Currency,
                        SourceCode = table.Table,
                        SourceName = table.Table,
                        EffectiveDate = table.EffectiveDate,
                        BuyPrice = rate.Bid,
                        SellPrice = rate.Ask
                    });
                }
            }

            return latestRates;
        }

        public async Task<bool> ExistsAsync(int currencyId, int rateSourceId, DateTime effectiveDate)
        {
            return await _dbSet.AnyAsync(er =>
                er.CurrencyId == currencyId &&
                er.RateSourceId == rateSourceId &&
                er.EffectiveDate == effectiveDate);
        }
    }
}