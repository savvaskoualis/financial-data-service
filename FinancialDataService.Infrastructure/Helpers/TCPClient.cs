using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDataService.Infrastructure.Helpers
{
    public class TCPClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly string _host;
        private readonly int _port;
        private readonly Func<string, Task> _onMessageReceived;
        private bool _disposed;

        public TCPClient(string host, int port, Func<string, Task> onMessageReceived = null)
        {
            _host = host;
            _port = port;
            _onMessageReceived = onMessageReceived;
            Task.Run(StartAsync);
        }

        private async Task ConnectAsync()
        {
            while (!_disposed)
            {
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(_host, _port);
                    _stream = _client.GetStream();
                    Console.WriteLine("Connected to TCP server.");
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Failed to connect to TCP server: {ex.Message}. Retrying in 1 second...");
                    await Task.Delay(1000);
                }
            }
        }

        public async Task StartAsync()
        {
            await ConnectAsync();

            if (_onMessageReceived != null)
            {
                var buffer = new byte[1024];
                while (!_disposed)
                {
                    try
                    {
                        var byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
                        if (byteCount == 0) break;
                        var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Console.WriteLine($"Received from server: {message}");
                        await _onMessageReceived(message);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"IOException: {ex.Message}");
                        await ReconnectAsync();
                    }
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_stream == null || !_client.Connected)
                {
                    await ReconnectAsync();
                }

                var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
                await _stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                Console.WriteLine($"Sent message: {message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException on SendMessageAsync: {ex.Message}");
                await ReconnectAsync();
                await SendMessageAsync(message);
            }
        }

        private async Task ReconnectAsync()
        {
            DisposeClient();
            await ConnectAsync();
        }

        private void DisposeClient()
        {
            _stream?.Dispose();
            _client?.Dispose();
            _stream = null;
            _client = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            DisposeClient();
        }
    }
}
