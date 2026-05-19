using GieudexPol.Application.DTOs;
using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;

namespace GieudexPol.Application.Services;

public class CurrencyExchangeSimulationService
    : ICurrencyExchangeSimulationService
{
    private readonly IExchangeRateService _exchangeRateService;

    public CurrencyExchangeSimulationService(
        IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    public async Task<CurrencyExchangeSimulationResponseDto>
        SimulateExchangeAsync(
        CurrencyExchangeSimulationRequestDto request)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Kwota musi być większa od zera.");
        }

        var exchangeRate =
            await _exchangeRateService.GetByCurrencyPairAsync(
                request.SourceCurrency,
                request.TargetCurrency);

        if (exchangeRate == null)
        {
            throw new Exception(
                "Nie znaleziono kursu walut.");
        }

        decimal exchangedAmount =
            request.Amount * exchangeRate.SellPrice;

        decimal feeAmount =
            exchangedAmount * (request.FeePercent / 100);

        decimal finalAmount =
            exchangedAmount + feeAmount;

        var simulation = new CurrencyExchangeSimulation
        {
            Amount = request.Amount,
            SourceCurrency = request.SourceCurrency,
            TargetCurrency = request.TargetCurrency,
            ExchangeRate = exchangeRate.SellPrice,
            FeePercent = request.FeePercent,
            FeeAmount = feeAmount,
            FinalAmount = finalAmount,
            SimulationDate = DateTime.UtcNow
        };

        return new CurrencyExchangeSimulationResponseDto
        {
            OriginalAmount = request.Amount,
            ExchangedAmount = exchangedAmount,

            SourceCurrency = request.SourceCurrency,
            TargetCurrency = request.TargetCurrency,

            ExchangeRate = simulation.ExchangeRate,
            FeeAmount = simulation.FeeAmount,
            FinalAmount = simulation.FinalAmount,
            SimulationDate = simulation.SimulationDate
        };
    }
}