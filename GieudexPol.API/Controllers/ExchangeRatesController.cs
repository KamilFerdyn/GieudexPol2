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
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRatesController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
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
        public async Task<IActionResult> GetLatestRates([FromQuery] string source = "MOCK_BANK_A")
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return BadRequest("Source query parameter is required.");
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
