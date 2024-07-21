using FinancialDataService.Application.Models;
using FinancialDataService.Domain.Interfaces;
using MediatR;

namespace FinancialDataService.Application.Queries
{
    public class GetFinancialInstrumentPriceQueryHandler : IRequestHandler<GetFinancialInstrumentPriceQuery, FinancialInstrumentPriceDto>
    {
        private readonly IFinancialInstrumentRepository _repository;

        public GetFinancialInstrumentPriceQueryHandler(IFinancialInstrumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<FinancialInstrumentPriceDto> Handle(GetFinancialInstrumentPriceQuery request, CancellationToken cancellationToken)
        {
            var instrument = await _repository.GetBySymbolAsync(request.Symbol.ToUpperInvariant());
            if (instrument == null)
            {
                return null;
            }

            return new FinancialInstrumentPriceDto
            {
                Symbol = instrument.Symbol,
                Price = instrument.LastTradePrice,
                LastUpdated = instrument.LastTradeTime
            };
        }
    }
}