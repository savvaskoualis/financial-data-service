using System.Text.Json.Serialization;

namespace FinancialDataService.Infrastructure.Models.Binance
{
    public class SubscriptionMessage
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public List<string> Params { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}