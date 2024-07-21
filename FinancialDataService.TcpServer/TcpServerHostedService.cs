using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialDataService.TcpServer
{
    public class TcpServerHostedService : IHostedService, IDisposable
    {
        private readonly TCPServer _tcpServer;

        public TcpServerHostedService(TCPServer tcpServer)
        {
            _tcpServer = tcpServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _tcpServer.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tcpServer.Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tcpServer.Dispose();
        }
    }
}