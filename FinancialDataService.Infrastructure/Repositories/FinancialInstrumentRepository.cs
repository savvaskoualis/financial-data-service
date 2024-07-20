using EFCore.BulkExtensions;
using FinancialDataService.Domain.Entities;
using FinancialDataService.Domain.Interfaces;
using FinancialDataService.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataService.Infrastructure.Repositories
{
    /// <inheritdoc cref="IFinancialInstrumentRepository"/>
    public class FinancialInstrumentRepository : IFinancialInstrumentRepository
    {
        private readonly FinancialServiceDbContext _context;
        private const int BatchSize = 1000;

        public FinancialInstrumentRepository(FinancialServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialInstrument>> GetAllAsync()
        {
            return await _context.FinancialInstruments.ToListAsync();
        }

        public async Task<int> UpdateInstrumentsAsync(IEnumerable<FinancialInstrument> instruments)
        {
            var existingInstruments = await _context.Set<FinancialInstrument>().ToListAsync();
            var existingSymbolsSet = existingInstruments.Select(sym => sym.Symbol).ToHashSet();

            var symbolsToInsert = instruments.Where(sym => !existingSymbolsSet.Contains(sym.Symbol)).ToList();

            foreach (var batch in symbolsToInsert.ChunkBy(BatchSize))
            {
                await _context.BulkInsertAsync(batch);
            }

            return symbolsToInsert.Count;
        }

        public async Task<FinancialInstrument> GetBySymbolAsync(string symbol)
        {
            return await _context.FinancialInstruments.FirstOrDefaultAsync(fi => fi.Symbol == symbol);
        }
    }
}