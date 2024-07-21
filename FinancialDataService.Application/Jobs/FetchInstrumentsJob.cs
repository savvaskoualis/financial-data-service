using FinancialDataService.Application.Interfaces;
using FinancialDataService.Domain.Entities;
using FinancialDataService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinancialDataService.Application.Jobs;

public class FetchInstrumentsJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FetchInstrumentsJob> _logger;

    public FetchInstrumentsJob(IServiceProvider serviceProvider, ILogger<FetchInstrumentsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var instrumentProvider = scope.ServiceProvider.GetRequiredService<IFinancialInstrumentProvider>();
            var instrumentRepository = scope.ServiceProvider.GetRequiredService<IFinancialInstrumentRepository>();

            try
            {
                _logger.LogInformation("Starting to fetch symbols from the market data service.");
                var symbols = (await instrumentProvider.GetAvailableInstrumentsAsync()).ToList();
                _logger.LogInformation($"Retrieved {symbols.Count()} symbols.");

                if (symbols.Any())
                {
                    var insertedSymbolsCount = await instrumentRepository.UpdateInstrumentsAsync(symbols.Select(s => new FinancialInstrument() { Symbol = s.Symbol, BaseAsset = s.BaseAsset, QuoteAsset = s.QuoteAsset }));

                    if (insertedSymbolsCount > 0)
                    {
                        _logger.LogInformation($"Successfully inserted {insertedSymbolsCount} new symbols in the database.");
                    }
                    else
                    {
                        _logger.LogInformation($"No new symbols were inserted.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching and updating symbols.");
            }
        }
    }
}