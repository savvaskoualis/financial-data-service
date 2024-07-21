using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FinancialDataService.Streams.Hubs;

public class PriceHub : Hub
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> Subscriptions = new();
    private readonly ILogger<PriceHub> _logger;

    public PriceHub(ILogger<PriceHub> logger)
    {
        _logger = logger;
    }

    public async Task SubscribeToPriceUpdates(string symbol)
    {
        var connectionId = Context.ConnectionId;
        if (!Subscriptions.ContainsKey(connectionId))
        {
            Subscriptions[connectionId] = new HashSet<string>();
        }
        Subscriptions[connectionId].Add(symbol);
        _logger.LogInformation("User with connection id {ConnectionId} subscribed to updates for {Symbol}", connectionId, symbol);
        await Clients.Caller.SendAsync("Subscribed", symbol);
    }

    public async Task UnsubscribeFromPriceUpdates(string symbol)
    {
        var connectionId = Context.ConnectionId;
        if (Subscriptions.ContainsKey(connectionId))
        {
            Subscriptions[connectionId].Remove(symbol);
            if (!Subscriptions[connectionId].Any())
            {
                Subscriptions.TryRemove(connectionId, out _);
            }
            _logger.LogInformation("User with connection id {ConnectionId} unsubscribed from updates for {Symbol}", connectionId, symbol);
            await Clients.Caller.SendAsync("Unsubscribed", symbol);
        }
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        Subscriptions.TryRemove(connectionId, out _);
        _logger.LogInformation("User with connection id {ConnectionId} disconnected.", connectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task LogAndSendPriceUpdate(IHubContext<PriceHub> hubContext, string symbol, decimal price, decimal quantity, DateTime tradeTime)
    {
        foreach (var subscription in Subscriptions)
        {
            if (subscription.Value.Contains(symbol))
            {
                await hubContext.Clients.Client(subscription.Key).SendAsync("ReceivePriceUpdate", symbol, price, quantity, tradeTime);
                _logger.LogInformation("Sent price update for {Symbol} to client {ClientId}", symbol, subscription.Key);
            }
        }
    }
    public static async Task SendPriceUpdate(IHubContext<PriceHub> hubContext, string symbol, decimal price, decimal quantity, DateTime tradeTime)
    {
        // Get hub from the context and invoke new method for sending updates
        var hub = hubContext as PriceHub;
        if(hub != null)
        {
            await hub.LogAndSendPriceUpdate(hubContext, symbol, price, quantity, tradeTime);
        }
    }
}