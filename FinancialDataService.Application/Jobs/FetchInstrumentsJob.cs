using FinancialDataService.Application.Interfaces;
using FinancialDataService.Domain.Entities;
using FinancialDataService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialDataService.Application.Jobs
{
    public class FetchInstrumentsJob
    {
        private readonly IFinancialInstrumentProvider _instrumentProvider;
        private readonly IFinancialInstrumentRepository _instrumentRepository;
        private readonly ILogger<FetchInstrumentsJob> _logger;

        public FetchInstrumentsJob(IFinancialInstrumentProvider instrumentProvider, IFinancialInstrumentRepository instrumentRepository, ILogger<FetchInstrumentsJob> logger)
        {
            _instrumentProvider = instrumentProvider;
            _instrumentRepository = instrumentRepository;
            _logger = logger;
        }

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting to fetch symbols from the market data service.");
                var symbols = (await _instrumentProvider.GetAvailableInstrumentsAsync()).ToList();
                _logger.LogInformation($"Retrieved {symbols.Count()} symbols.");

                if (symbols.Any())
                {
                    var insertedSymbolsCount = await _instrumentRepository.UpdateInstrumentsAsync(symbols.Select(s => new FinancialInstrument() { Symbol = s.Symbol, BaseAsset = s.BaseAsset, QuoteAsset = s.QuoteAsset }));

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