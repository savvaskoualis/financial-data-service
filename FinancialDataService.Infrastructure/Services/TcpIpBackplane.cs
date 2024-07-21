using FinancialDataService.Application.Interfaces;
using FinancialDataService.Infrastructure.Helpers;
using FinancialDataService.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FinancialDataService.Infrastructure.Services
{
    public class TcpIpBackplane : IBackplane, IDisposable
    {
        private TCPClient _client;
        private readonly string _host;
        private readonly int _port;
        private bool _disposed;

        public TcpIpBackplane(IOptions<TcpBackplaneSettings> settings)
        {
            var config = settings.Value;
            _host = config.Host;
            _port = config.Port;
            StartSimpleClientAsync().Wait(); // Start the client when the backplane is instantiated
        }

        private async Task StartSimpleClientAsync()
        {
            _client = new TCPClient(_host, _port);
            await _client.StartAsync();
        }

        public async Task PublishAsync(string message)
        {
            if (_client == null)
            {
                await StartSimpleClientAsync();
            }

            await _client.SendMessageAsync(message);
        }

        public async Task SubscribeAsync(Func<string, Task> onMessageReceived)
        {
            _client = new TCPClient(_host, _port, onMessageReceived);
            await _client.StartAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _client?.Dispose();
        }
    }
}