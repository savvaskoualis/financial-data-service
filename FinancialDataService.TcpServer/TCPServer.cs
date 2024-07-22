using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialDataService.TcpServer
{
    public class TCPServer : IDisposable
    {
        private readonly ConcurrentBag<TcpClient> _clients = new ConcurrentBag<TcpClient>();
        private readonly TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public TCPServer(IPAddress address, int port)
        {
            _listener = new TcpListener(address, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("TCP Server started...");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _clients.Add(client);
                    Console.WriteLine("Client connected...");
                    _ = HandleClientAsync(client);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var buffer = new byte[1024];
            var receivedData = new StringBuilder();
            using (var stream = client.GetStream())
            {
                while (true)
                {
                    try
                    {
                        var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (byteCount == 0) break;

                        receivedData.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));

                        string data = receivedData.ToString();
                        int delimiterIndex;
                        while ((delimiterIndex = data.IndexOf('\n')) != -1)
                        {
                            var message = data.Substring(0, delimiterIndex).Trim();
                            data = data.Substring(delimiterIndex + 1);
                            Console.WriteLine($"Received message: {message}");
                            BroadcastMessage(message);
                        }

                        receivedData.Clear();
                        receivedData.Append(data);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"IOException: {ex.Message}");
                        break;
                    }
                }
            }

            _clients.TryTake(out _);
        }

        private void BroadcastMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            foreach (var client in _clients)
            {
                try
                {
                    var stream = client.GetStream();
                    stream.Write(messageBytes, 0, messageBytes.Length);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IOException on broadcast: {ex.Message}");
                    client.Close();
                    _clients.TryTake(out _);
                }
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
            foreach (var client in _clients)
            {
                client.Close();
            }
            Console.WriteLine("TCP Server stopped.");
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource.Dispose();
        }
    }
}
