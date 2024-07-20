using FinancialDataService.Application.Models;

namespace FinancialDataService.Application.Interfaces
{
    public interface IStreamingPriceDataProvider
    {
        event Action<PriceUpdateModel> OnPriceUpdate;

        Task StartAsync(List<string> instrumentSymbols);
        Task StopAsync();
    }
}