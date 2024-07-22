using FinancialDataService.Application.Interfaces;
using FinancialDataService.Infrastructure.Helpers;
using FinancialDataService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FinancialDataService.Infrastructure.Services
{
    public class TcpIpBackplane : IBackplane, IDisposable
    {
        private TCPClient _publishClient;
        private TCPClient _subscribeClient;
        private readonly string _host;
        private readonly int _port;
        private bool _disposed;

        public TcpIpBackplane(IOptions<TcpBackplaneSettings> settings)
        {
            var config = settings.Value;
            _host = config.Host;
            _port = config.Port;
            StartPublishClientAsync().Wait();
        }

        private async Task StartPublishClientAsync()
        {
            _publishClient = new TCPClient(_host, _port);
            await _publishClient.StartAsync();
        }

        public async Task PublishAsync(string message)
        {
            if (_publishClient == null)
            {
                await StartPublishClientAsync();
            }

            await _publishClient.SendMessageAsync(message);
        }

        public async Task SubscribeAsync(Func<string, Task> onMessageReceived)
        {
            if (_subscribeClient != null)
            {
                _subscribeClient.Dispose();
            }

            _subscribeClient = new TCPClient(_host, _port, onMessageReceived);
            await _subscribeClient.StartAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _publishClient?.Dispose();
            _subscribeClient?.Dispose();
        }
    }
}