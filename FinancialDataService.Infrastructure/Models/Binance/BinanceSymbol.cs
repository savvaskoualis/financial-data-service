using System.Text.Json.Serialization;

namespace FinancialDataService.Infrastructure.Models.Binance
{
    public class BinanceSymbol
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("baseAsset")]
        public string BaseAsset { get; set; }

        [JsonPropertyName("quoteAsset")]
        public string QuoteAsset { get; set; }
    }
}