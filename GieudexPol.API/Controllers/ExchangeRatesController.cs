using GieudexPol.Application.Interfaces;
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
    }
}