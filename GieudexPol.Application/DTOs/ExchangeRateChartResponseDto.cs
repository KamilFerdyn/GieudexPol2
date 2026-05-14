namespace GieudexPol.Application.DTOs
{
    public class ExchangeRateChartResponseDto
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public string SourceCode { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<ExchangeRateChartPointDto> Points { get; set; } = new List<ExchangeRateChartPointDto>();
    }
}
