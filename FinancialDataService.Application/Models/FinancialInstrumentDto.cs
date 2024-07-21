namespace FinancialDataService.Application.Models
{
    public class FinancialInstrumentDto
    {
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
    }
}