using FinancialDataService.Application.Models;
using FinancialDataService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinancialDataService.Api.Controllers;

[ApiController]
[ApiVersion(version: "1")]
[Route("api/v{version:apiVersion}/financialinstruments")]
public class FinancialInstrumentController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialInstrumentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a list of available instruments
    /// </summary>
    /// <response code="200">Returns the list of available financial instruments</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialInstrumentDto>>> GetAvailableInstruments()
    {
        var query = new GetAvailableFinancialInstrumentsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get instrument price by symbol
    /// </summary>
    /// <param name="symbol">Instrument symbol</param>
    /// <response code="200">Returns the price of the financial instrument</response>
    /// <response code="404">If the price of the instrument is not found</response>
    [HttpGet("{symbol}")]
    public async Task<ActionResult<FinancialInstrumentPriceDto>> GetInstrumentPrice(string symbol)
    {
        var query = new GetFinancialInstrumentPriceQuery(symbol);
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}