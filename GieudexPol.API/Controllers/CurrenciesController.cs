using GieudexPol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GieudexPol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var currencies = await _currencyService.GetAllAsync();
            return Ok(currencies);
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetCurrencyBySymbol(string symbol)
        {
            var currency = await _currencyService.GetBySymbolAsync(symbol);
            if (currency == null)
            {
                return NotFound();
            }
            return Ok(currency);
        }
    }
}