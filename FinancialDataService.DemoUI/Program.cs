using FinancialDataService.DemoUI;
using FinancialDataService.UI;
using FinancialDataService.UI.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Load appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<AppSettings>(options =>
    builder.Configuration.GetSection("Connections").Bind(options));

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    return new HttpClient { BaseAddress = new Uri(settings.ApiBaseUrl) };
});
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    return new HubConnectionBuilder().WithUrl(settings.HubUrl).Build();
});

builder.Services.AddScoped<WebSocketStateService>();


builder.Services.AddMudServices();

await builder.Build().RunAsync();