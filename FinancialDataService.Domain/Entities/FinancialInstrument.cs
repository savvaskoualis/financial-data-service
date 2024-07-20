namespace FinancialDataService.Domain.Entities
{
    public class FinancialInstrument
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public decimal LastTradePrice { get; set; }
        public decimal LastTradeQuantity { get; set; }
        public DateTime LastTradeTime { get; set; }
    }
}