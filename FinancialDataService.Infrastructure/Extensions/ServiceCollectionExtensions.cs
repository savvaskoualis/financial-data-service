using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Jobs;
using FinancialDataService.Domain.Interfaces;
using FinancialDataService.Infrastructure.Options;
using FinancialDataService.Infrastructure.Repositories;
using FinancialDataService.Infrastructure.Scheduling;
using FinancialDataService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace FinancialDataService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddGlobalProviderSettings(configuration);
            services.AddBinanceSettings(configuration);
            services.AddQuartzJobs(configuration);

            services.AddScoped<IFinancialInstrumentRepository, FinancialInstrumentRepository>();
            services.AddScoped<IFinancialInstrumentProvider, BinanceInstrumentsProvider>();
            services.AddSingleton<IStreamingPriceDataProvider, BinanceStreamingPriceDataProvider>();

            services.AddHostedService<LivePricesBackgroundService>();

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<FinancialServiceDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));


            return services;
        }

        private static void AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<FetchInstrumentsJob>();
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("FetchInstrumentsJob");

                q.AddJob<QuartzInstrumentsJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("FetchSymbolsJob-trigger")
                    .WithCronSchedule(configuration["GlobalProviderSettings:FetchInstrumentsJobCronExpression"] ?? "0 0 * * * ?")); // fallback to every hour
            });

            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }

        /// <summary>
        /// Registers the Binance settings in the DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the settings to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance containing the configuration data.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddBinanceSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BinanceSettings>(configuration.GetSection("BinanceSettings"));

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<BinanceSettings>>().Value);

            services.AddHttpClient("BinanceClient", client =>
            {
                client.BaseAddress = new Uri(configuration["BinanceSettings:RestApiBaseUrl"] ?? throw new InvalidOperationException());
            });

            return services;
        }

        /// <summary>
        /// Registers the global provider settings in the DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the settings to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance containing the configuration data.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddGlobalProviderSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProviderSettings>(configuration.GetSection("GlobalProviderSettings"));

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ProviderSettings>>().Value);


            return services;
        }
    }
}