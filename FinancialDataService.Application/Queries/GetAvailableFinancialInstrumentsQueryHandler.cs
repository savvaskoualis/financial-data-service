using FinancialDataService.Application.Models;
using FinancialDataService.Domain.Interfaces;
using MediatR;

namespace FinancialDataService.Application.Queries
{
    public class GetAvailableFinancialInstrumentsHandler(IFinancialInstrumentRepository repository) : IRequestHandler<GetAvailableFinancialInstrumentsQuery, List<FinancialInstrumentDto>>
    {
        private readonly IFinancialInstrumentRepository _repository = repository;

        public async Task<List<FinancialInstrumentDto>> Handle(GetAvailableFinancialInstrumentsQuery request, CancellationToken cancellationToken)
        {
            var instruments = await _repository.GetAllAsync();
            return instruments.Select(i => new FinancialInstrumentDto
            {
                Symbol = i.Symbol,
                BaseAsset = i.BaseAsset,
                QuoteAsset = i.QuoteAsset
            }).ToList();
        }
    }
}