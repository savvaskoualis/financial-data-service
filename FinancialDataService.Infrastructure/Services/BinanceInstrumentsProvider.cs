using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Models;
using FinancialDataService.Infrastructure.Models.Binance;
using FinancialDataService.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Web;

namespace FinancialDataService.Infrastructure.Services
{
    public class BinanceInstrumentsProvider : IFinancialInstrumentProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ProviderSettings> _globalProviderOptions;
        private readonly IOptions<BinanceSettings> _binanceSettings;
        private readonly ILogger<BinanceInstrumentsProvider> _logger;

        public BinanceInstrumentsProvider(IHttpClientFactory httpClientFactory, IOptions<ProviderSettings> globalProviderOptions, ILogger<BinanceInstrumentsProvider> logger, IOptions<BinanceSettings> binanceSettings)
        {
            _httpClient = httpClientFactory.CreateClient("BinanceClient");
            _globalProviderOptions = globalProviderOptions;
            _logger = logger;
            _binanceSettings = binanceSettings;
        }

        public async Task<IEnumerable<InstrumentModel>> GetAvailableInstrumentsAsync()
        {
            _logger.LogInformation("Retrieving available instruments...");

            var supportedSymbols = _globalProviderOptions.Value.SupportedSymbols;

            var response = await _httpClient.GetAsync($"{_binanceSettings.Value.RestApiBaseUrl}/exchangeInfo");
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully retrieved available instruments data from the Binance API.");

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<BinanceExchangeInfo>(jsonString);

            // filtering the supported symbols here instead of passing the symbols param to the API for simplicity
            // since binance endpoint fails when provided with an invalid/unsupported symbol
            var result = data?.Symbols.Where(s => supportedSymbols.Contains(s.Symbol)).Select(s => new InstrumentModel()
            {
                Symbol = s.Symbol,
                BaseAsset = s.BaseAsset,
                QuoteAsset = s.QuoteAsset
            }) ?? [];

            _logger.LogInformation("Filtered {Count} supported instruments ({Instruments}).", result.Count(), string.Join(", ", result.Select(i => i.Symbol)));

            return result;
        }
    }
}