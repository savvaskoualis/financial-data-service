using System;
using System.Threading.Tasks;
using FinancialDataService.Application.Interfaces;
using FinancialDataService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace FinancialDataService.Infrastructure.Services
{
    public class RedisBackplane : IBackplane, IDisposable
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisBackplane> _logger;
        private readonly ISubscriber _subscriber;
        private bool _disposed;

        public RedisBackplane(IOptions<RedisBackplaneSettings> settings, ILogger<RedisBackplane> logger)
        {
            _redis = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
            _subscriber = _redis.GetSubscriber();
            _logger = logger;
        }

        public async Task PublishAsync(string message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RedisBackplane));

            await _subscriber.PublishAsync("price-updates", message);
        }

        public async Task SubscribeAsync(Func<string, Task> onMessageReceived)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RedisBackplane));

            await _subscriber.SubscribeAsync("price-updates", async (channel, message) =>
            {
                _logger.LogInformation($"Received message from Redis: {message}");
                await onMessageReceived(message);
            });
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _redis?.Dispose();
        }
    }
}