using Microsoft.AspNetCore.SignalR.Client;

namespace FinancialDataService.UI.Services;

public class WebSocketStateService
{
    public List<Subscriber> Subscribers { get; private set; } = new List<Subscriber>();

    public class Subscriber
    {
        public string Symbol { get; set; }
        public decimal LatestPrice { get; set; }
        public decimal LatestQuantity { get; set; }
        public DateTime LastTradeTime { get; set; }
        public bool IsHighlighted { get; set; }
        public HubConnection Connection { get; set; }
    }
}