using FinancialDataService.Application.Models;
using MediatR;

namespace FinancialDataService.Application.Queries;

public class GetAvailableFinancialInstrumentsQuery : IRequest<List<FinancialInstrumentDto>>
{
}