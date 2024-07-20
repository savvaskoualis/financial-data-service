using FinancialDataService.Domain.Entities;

namespace FinancialDataService.Domain.Interfaces
{
    /// <summary>
    /// Represents a repository for managing financial instruments.
    /// </summary>
    public interface IFinancialInstrumentRepository
    {
        /// <summary>
        /// Retrieves all financial instruments asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a collection of financial instruments.
        /// </returns>
        Task<IEnumerable<FinancialInstrument>> GetAllAsync();

        /// <summary>
        /// Updates the financial instruments by inserting new symbols into the database.
        /// </summary>
        /// <param name="instruments">The list of financial instruments to update.</param>
        /// <returns>The number of new symbols inserted into the database.</returns>
        Task<int> UpdateInstrumentsAsync(IEnumerable<FinancialInstrument> instruments);

        /// <summary>
        /// Retrieves a financial instrument by its symbol asynchronously.
        /// </summary>
        /// <param name="priceUpdateSymbol">The symbol of the financial instrument to retrieve.</param>
        /// <returns>The financial instrument with the specified symbol, or null if not found.</returns>
        Task<FinancialInstrument> GetBySymbolAsync(string priceUpdateSymbol);
    }
}