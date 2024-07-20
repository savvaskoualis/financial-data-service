using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Models;
using FinancialDataService.Infrastructure.Models.Binance;
using FinancialDataService.Infrastructure.Models.Binance.FinancialDataService.Application.Models;
using FinancialDataService.Infrastructure.Options;
using FinancialDataService.Shared.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace FinancialDataService.Infrastructure.Services
{
    public class BinanceStreamingPriceDataProvider : IStreamingPriceDataProvider
    {
        private readonly ILogger<BinanceStreamingPriceDataProvider> _logger;
        private readonly IOptions<BinanceSettings> _binanceOptions;
        private ClientWebSocket _client;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _receivingTask;
        private int _subscriptionId = 1;

        public event Action<PriceUpdateModel> OnPriceUpdate;

        public BinanceStreamingPriceDataProvider(ILogger<BinanceStreamingPriceDataProvider> logger, IOptions<BinanceSettings> binanceOptions)
        {
            _logger = logger;
            _binanceOptions = binanceOptions;
        }

        public async Task StartAsync(List<string> tradingPairs)
        {
            _client = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            string streamsParameter = string.Join("/", tradingPairs);
            Uri webSocketUri = new Uri(_binanceOptions.Value.WebSocketBaseUrl + "?streams=" + streamsParameter);
            await _client.ConnectAsync(webSocketUri, _cancellationTokenSource.Token);

            // Subscribe to trading pairs in batches
            const int batchSize = 1000;
            for (int i = 0; i < tradingPairs.Count; i += batchSize)
            {
                var batch = tradingPairs.GetRange(i, Math.Min(batchSize, tradingPairs.Count - i));
                await SubscribeToTradingPairs(batch);
            }

            _receivingTask = ReceiveMessagesAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            if (_client != null)
            {
                await _cancellationTokenSource.CancelAsync();
                await _receivingTask;
                _client.Dispose();
            }
        }

        private async Task SubscribeToTradingPairs(List<string> tradingPairs)
        {
            var subscribeMessage = new SubscriptionMessage
            {
                Method = "SUBSCRIBE",
                Params = tradingPairs.Select(t => $"{t.ToLowerInvariant()}@aggTrade").ToList(),
                Id = _subscriptionId++
            };

            var messageJson = JsonSerializer.Serialize(subscribeMessage);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var messageSegment = new ArraySegment<byte>(messageBytes);
            await _client.SendAsync(messageSegment, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }

        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                var message = new StringBuilder();

                do
                {
                    result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                        return;
                    }
                    message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                ProcessMessage(message.ToString());
            }
        }

        private void ProcessMessage(string message)
        {
            // Check if the message is a subscription confirmation message
            if (message.Contains("\"result\":") && message.Contains("\"id\":"))
            {
                _logger.LogInformation("Received subscription confirmation message: {Message}", message);
                return;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new UnixEpochDateTimeConverter(),
                        new StringToDecimalConverter()
                    }
                };

                var streamData = JsonSerializer.Deserialize<BinanceStreamData>(message, options);
                var tradeData = streamData.Data;
                var priceUpdate = new PriceUpdateModel
                {
                    Symbol = tradeData.Symbol,
                    Price = tradeData.Price,
                    Quantity = tradeData.Quantity,
                    TradeTime = tradeData.TradeTime
                };

                OnPriceUpdate?.Invoke(priceUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from WebSocket");
            }
        }
    }
}