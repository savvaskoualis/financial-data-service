using FinancialDataService.DemoUI;
using FinancialDataService.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();