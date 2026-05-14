using System.Text.Json.Serialization;

namespace GieudexPol.Application.DTOs
{
    public class NbpExchangeRateTableDto
    {
        [JsonPropertyName("table")]
        public string Table { get; set; } = string.Empty;

        [JsonPropertyName("no")]
        public string No { get; set; } = string.Empty;

        [JsonPropertyName("effectiveDate")]
        public DateTime EffectiveDate { get; set; }

        [JsonPropertyName("rates")]
        public List<NbpExchangeRateItemDto> Rates { get; set; } = new List<NbpExchangeRateItemDto>();
    }
}
