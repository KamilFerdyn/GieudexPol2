namespace GieudexPol.Application.DTOs;

public class CurrencyExchangeSimulationResponseDto
{
    public decimal ExchangeRate { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public DateTime SimulationDate { get; set; }
}