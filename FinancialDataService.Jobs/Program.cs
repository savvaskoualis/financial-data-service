using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Jobs;
using FinancialDataService.Infrastructure.Extensions;
using FinancialDataService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();

        var env = hostingContext.HostingEnvironment;

        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        configuration.AddEnvironmentVariables();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructureServices(context.Configuration);
        services.AddStreams(context.Configuration);
        services.AddJobs(context.Configuration);
    })
    .Build();

// Run instrument fetching job once to populate symbols in DB
using(var serviceScope = host.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var job = services.GetRequiredService<FetchInstrumentsJob>();
    await job.Execute();
}

await host.RunAsync();