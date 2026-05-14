using System.Text.Json.Serialization;

namespace GieudexPol.Application.DTOs
{
    public class NbpExchangeRateItemDto
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("bid")]
        public decimal Bid { get; set; }

        [JsonPropertyName("ask")]
        public decimal Ask { get; set; }
    }
}
