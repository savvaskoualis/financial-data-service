using FinancialDataService.Application.Interfaces;
using FinancialDataService.Streams.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace FinancialDataService.Streams
{
    public class PriceUpdateHandler
    {
        private readonly IBackplane _backplane;
        private readonly IHubContext<PriceHub> _hubContext;

        public PriceUpdateHandler(IBackplane backplane, IHubContext<PriceHub> hubContext)
        {
            _backplane = backplane;
            _hubContext = hubContext;
            Task.Run(() => _backplane.SubscribeAsync(OnPriceUpdateReceived));
        }

        private async Task OnPriceUpdateReceived(string message)
        {
            var parts = message.Split('|');
            var symbol = parts[0];
            var price = decimal.Parse(parts[1]);
            var quantity = decimal.Parse(parts[2]);
            var tradeTime = DateTime.Parse(parts[3], null, System.Globalization.DateTimeStyles.RoundtripKind);


            await _hubContext.Clients.All.SendAsync("ReceivePriceUpdate", symbol, price, quantity, tradeTime);
        }

        public async Task PublishPriceUpdate(string symbol, decimal price, decimal quantity, DateTime tradeTime)
        {
        }
    }
}