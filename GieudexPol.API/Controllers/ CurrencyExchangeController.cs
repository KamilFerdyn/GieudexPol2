using GieudexPol.Application.DTOs;
using GieudexPol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GieudexPol.API.Controllers;

[ApiController]
[Route("api/exchange")]
public class ExchangeController : ControllerBase
{
    private readonly ICurrencyExchangeSimulationService _simulationService;

    public ExchangeController(
        ICurrencyExchangeSimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    // [HttpPost("calculate")]
    // public async Task<IActionResult> CalculateExchange(
    //     [FromBody] CurrencyExchangeSimulationRequestDto request)
    // {
    //     try
    //     {
    //         var result =
    //             await _simulationService
    //                 .SimulateExchangeAsync(request);

    //         return Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new
    //         {
    //             message = ex.Message
    //         });
    //     }
    // }

    [HttpPost("calculate")]
    public IActionResult CalculateExchange(
    [FromBody] CurrencyExchangeSimulationRequestDto request)
    {
        return Ok(new
        {
            convertedAmount = 123,
            feeAmount = 5,
            finalAmount = 118
        });
    }
}