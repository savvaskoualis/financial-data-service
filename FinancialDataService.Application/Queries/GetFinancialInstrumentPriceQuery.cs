using FinancialDataService.Application.Models;
using MediatR;

namespace FinancialDataService.Application.Queries;

public class GetFinancialInstrumentPriceQuery : IRequest<FinancialInstrumentPriceDto>
{
    public string Symbol { get; set; }

    public GetFinancialInstrumentPriceQuery(string symbol)
    {
        Symbol = symbol;
    }
}