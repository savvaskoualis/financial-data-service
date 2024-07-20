using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Models;
using FinancialDataService.Infrastructure.Models.Binance;
using FinancialDataService.Infrastructure.Options;
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

        public BinanceInstrumentsProvider(IHttpClientFactory httpClientFactory, IOptions<ProviderSettings> globalProviderOptions)
        {
            _httpClient = httpClientFactory.CreateClient("BinanceClient");
            _globalProviderOptions = globalProviderOptions;
        }

        public async Task<IEnumerable<InstrumentModel>> GetAvailableInstrumentsAsync()
        {
            var supportedSymbols = _globalProviderOptions.Value.SupportedSymbols;

            var response = await _httpClient.GetAsync($"https://api.binance.com/api/v3/exchangeInfo");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<BinanceExchangeInfo>(jsonString);

            // filtering the supported symbols here instead of passing the symbols param to the API for simplicity
            // since binance endpoint fails when provided with an invalid/unsupported symbol
            return data?.Symbols.Where(s => supportedSymbols.Contains(s.Symbol)).Select(s => new InstrumentModel()
            {
                Symbol = s.Symbol,
                BaseAsset = s.BaseAsset,
                QuoteAsset = s.QuoteAsset
            }) ?? [];
        }
    }
}