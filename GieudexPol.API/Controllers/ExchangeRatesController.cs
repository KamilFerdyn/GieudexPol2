using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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