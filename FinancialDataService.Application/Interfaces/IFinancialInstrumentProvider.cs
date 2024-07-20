using FinancialDataService.Application.Models;

namespace FinancialDataService.Application.Interfaces
{
    public interface IFinancialInstrumentProvider
    {
        Task<IEnumerable<InstrumentModel>> GetAvailableInstrumentsAsync();
    }
}