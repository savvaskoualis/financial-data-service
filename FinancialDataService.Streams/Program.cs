using FinancialDataService.Application.Interfaces;
using FinancialDataService.Infrastructure.Extensions;
using FinancialDataService.Infrastructure.Services;
using FinancialDataService.Streams;
using FinancialDataService.Streams.Handlers;
using FinancialDataService.Streams.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddStreams(builder.Configuration);
builder.Services.AddSingleton<PriceUpdateHandler>();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<PriceHub>("/priceHub");
});

// Request the PriceUpdateHandler explicitly
app.Services.GetRequiredService<PriceUpdateHandler>();

app.Run();