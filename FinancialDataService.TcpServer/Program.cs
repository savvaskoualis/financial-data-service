using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using FinancialDataService.TcpServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register the TCP server and the hosted service
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var host = config.GetValue<string>("TcpServer:Host");
    var port = config.GetValue<int>("TcpServer:Port");
    return new TCPServer(IPAddress.Parse(host), port);
});
builder.Services.AddHostedService<TcpServerHostedService>();

var app = builder.Build();

app.Run();