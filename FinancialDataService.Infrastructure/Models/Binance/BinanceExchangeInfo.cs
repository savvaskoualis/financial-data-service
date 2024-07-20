using System.Text.Json.Serialization;

namespace FinancialDataService.Infrastructure.Models.Binance
{
    public class BinanceExchangeInfo
    {
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("serverTime")]
        public long ServerTime { get; set; }

        [JsonPropertyName("symbols")]
        public List<BinanceSymbol> Symbols { get; set; }
    }
}