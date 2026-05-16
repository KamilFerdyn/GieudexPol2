using System;
using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GieudexPol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private static readonly DateTime MinimumSyncDate = new DateTime(2026, 1, 1);

        private readonly IExchangeRateService _exchangeRateService;
        private readonly IExchangeRateSyncService _exchangeRateSyncService;

        public ExchangeRatesController(
            IExchangeRateService exchangeRateService,
            IExchangeRateSyncService exchangeRateSyncService)
        {
            _exchangeRateService = exchangeRateService;
            _exchangeRateSyncService = exchangeRateSyncService;
        }

        [HttpGet("{baseCurrencySymbol}/{targetCurrencySymbol}")]
        public async Task<IActionResult> GetExchangeRateByCurrencyPair(string baseCurrencySymbol, string targetCurrencySymbol)
        {
            var exchangeRate = await _exchangeRateService.GetByCurrencyPairAsync(baseCurrencySymbol, targetCurrencySymbol);
            if (exchangeRate == null)
            {
                return NotFound();
            }
            return Ok(exchangeRate);
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetChartData(
            [FromQuery] string currency,
            [FromQuery] string source,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                return BadRequest("Currency query parameter is required.");
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return BadRequest("Source query parameter is required.");
            }

            if (from > to)
            {
                return BadRequest("From date cannot be later than to date.");
            }

            var chartData = await _exchangeRateService.GetChartDataAsync(
                currency.Trim().ToUpperInvariant(),
                source.Trim().ToUpperInvariant(),
                from.Date,
                to.Date);

            return Ok(chartData);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string source = "NBP")
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                source = "NBP";
            }

            var rates = await _exchangeRateService.GetLatestRatesAsync(source.Trim().ToUpperInvariant());
            return Ok(rates);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExchangeRates()
        {
            var exchangeRates = await _exchangeRateService.GetAllAsync();
            return Ok(exchangeRates);
        }

        [HttpPost("sync/nbp")]
        public async Task<IActionResult> SyncNbpRates(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken cancellationToken)
        {
            if (from.Date > to.Date)
            {
                return BadRequest("From date cannot be later than to date.");
            }

            if (from.Date < MinimumSyncDate)
            {
                return BadRequest("NBP sync from date cannot be earlier than 2026-01-01.");
            }

            if (to.Date > DateTime.Today)
            {
                return BadRequest("NBP sync to date cannot be later than today.");
            }

            var result = await _exchangeRateSyncService.SyncNbpRatesAsync(
                from.Date,
                to.Date,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExchangeRate([FromBody] ExchangeRate exchangeRate)
        {
            await _exchangeRateService.AddAsync(exchangeRate);
            return CreatedAtAction(nameof(GetExchangeRateByCurrencyPair), new { id = exchangeRate.Id }, exchangeRate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExchangeRate(int id, [FromBody] ExchangeRate exchangeRate)
        {
            if (id != exchangeRate.Id)
            {
                return BadRequest();
            }
            await _exchangeRateService.UpdateAsync(exchangeRate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExchangeRate(int id)
        {
            var exchangeRate = await _exchangeRateService.GetByIdAsync(id);
            if (exchangeRate == null)
            {
                return NotFound();
            }
            await _exchangeRateService.DeleteAsync(exchangeRate);
            return NoContent();
        }
    }
}