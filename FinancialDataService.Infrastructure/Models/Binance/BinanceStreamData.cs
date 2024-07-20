namespace FinancialDataService.Infrastructure.Models.Binance
{
    using System.Text.Json.Serialization;

    namespace FinancialDataService.Application.Models
    {
        public class BinanceStreamData
        {
            [JsonPropertyName("stream")]
            public string Stream { get; set; }

            [JsonPropertyName("data")]
            public BinanceEventModel Data { get; set; }
        }
    }

}