using FinancialDataService.Shared.Converters;
using System.Text.Json.Serialization;

namespace FinancialDataService.Infrastructure.Models.Binance;

public class BinanceEventModel
{
    [JsonPropertyName("e")]
    public string EventType { get; set; }

    [JsonPropertyName("E")]
    [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime EventTime { get; set; }

    [JsonPropertyName("s")]
    public string Symbol { get; set; }

    [JsonPropertyName("a")]
    public long AggregateTradeId { get; set; }

    [JsonPropertyName("p")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal Price { get; set; }

    [JsonPropertyName("q")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal Quantity { get; set; }

    [JsonPropertyName("f")]
    public long FirstTradeId { get; set; }

    [JsonPropertyName("l")]
    public long LastTradeId { get; set; }

    [JsonPropertyName("T")]
    [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime TradeTime { get; set; }

    [JsonPropertyName("m")]
    public bool IsBuyerMarketMaker { get; set; }
}