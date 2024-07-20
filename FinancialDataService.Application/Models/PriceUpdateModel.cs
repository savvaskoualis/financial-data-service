namespace FinancialDataService.Application.Models
{
    public class PriceUpdateModel
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime TradeTime { get; set; }
    }
}