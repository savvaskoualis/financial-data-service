namespace FinancialDataService.Application.Models
{
    public class FinancialInstrumentPriceDto
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}