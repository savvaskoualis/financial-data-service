using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDataService.Infrastructure.Helpers
{
    public class TCPClient : IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _host;
        private readonly int _port;
        private readonly Func<string, Task> _onMessageReceived;
        private bool _disposed;
        private NetworkStream _stream;

        public TCPClient(string host, int port, Func<string, Task> onMessageReceived = null)
        {
            _client = new TcpClient();
            _host = host;
            _port = port;
            _onMessageReceived = onMessageReceived;
            Task.Run(StartAsync);
        }

        private async Task ConnectAsync()
        {
            while (!_client.Connected)
            {
                try
                {
                    await _client.ConnectAsync(_host, _port);
                    _stream = _client.GetStream();
                }
                catch (SocketException)
                {
                    await Task.Delay(1000); // Retry after delay
                }
            }
        }

        public async Task StartAsync()
        {
            await ConnectAsync();

            if (_onMessageReceived != null)
            {
                var buffer = new byte[1024];
                var receivedData = new StringBuilder();
                try
                {
                    while (!_disposed)
                    {
                        var byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
                        if (byteCount == 0) break;
                        receivedData.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));

                        string data = receivedData.ToString();
                        int delimiterIndex;
                        while ((delimiterIndex = data.IndexOf('\n')) != -1)
                        {
                            var message = data.Substring(0, delimiterIndex).Trim();
                            data = data.Substring(delimiterIndex + 1);
                            await _onMessageReceived(message);
                        }

                        receivedData.Clear();
                        receivedData.Append(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (!_client.Connected || _stream == null)
                {
                    await ReconnectAsync();
                }

                var messageBytes = Encoding.UTF8.GetBytes(message + "\n"); // Add delimiter
                await _stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
            catch (IOException)
            {
                await ReconnectAsync();
                await SendMessageAsync(message);
            }
        }

        private async Task ReconnectAsync()
        {
            Console.WriteLine("Reconnecting to TCP server...");
            Dispose();
            await ConnectAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _stream?.Dispose();
            _client?.Close();
            _client?.Dispose();
        }
    }
}
