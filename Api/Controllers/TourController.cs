using Application.Features.Tour.Commands;
using Application.Features.Tour.Queries;
using MassTransit.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OutputCaching;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController(ILogger<TourController> logger, MediatR.IMediator mediator)
    : BaseController
{
    [HttpGet]
    [OutputCache]
    [EnableQuery]
    public async Task<IQueryable<TourTemplateResponse>> Get()
    {
        return await mediator.Send(new GetTours());
    }

    // GET: api/Tour/5
    [HttpGet("{id}")]
    [OutputCache]
    public async Task<ActionResult<TourDetailResponse>> GetTours(Guid id)
    {
        var result = await mediator.Send(new GetTourDetail(id));

        return result.Match<ActionResult<TourDetailResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Tour ({id}) not found."));
    }


    [HttpPost]
    public async Task<IActionResult> CreateTour([FromBody] CreateTourCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetListTour()
    {
        var response = await mediator.Send(new GetListTour());
        return HandleServiceResult(response);
    }

    [HttpPut("tourinfor/{id}")]
    public async Task<IActionResult> PutTour(Guid id,[FromBody] UpdateTourInforCommand command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);

    }

    [HttpPut("tourdestination/{id}")]
    public async Task<IActionResult> UpdateTourDestination(Guid id,[FromBody] UpdateTourDestinationCommand command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);
    }

    [HttpDelete("tourschedule/{id}")]
    public async Task<IActionResult> DeleteTourSchedule(Guid id, [FromBody] DeleteTourSchedule command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);

    }

    [HttpPost("addschedule/{id}")]
    public async Task<IActionResult> AddSchedule(Guid id, [FromBody] AddScheduleCommand command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);
    }

    [HttpPut("ticketschedule/{id}")]
    public async Task<IActionResult> UpdateTicketSchedule(Guid id,[FromBody] UpdateTicketScheduleCommand command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);
    }
    [HttpGet("tourinfor/{id}")]
    public async Task<IActionResult> GetTourInforByTourID(Guid id)
    {
        var response = await mediator.Send(new GetTourInforByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    [HttpGet("tourdestination/{id}")]
    public async Task<IActionResult> GetTourDestinationByTourID(Guid id)
    {
        var response = await mediator.Send(new GetTourDestinationByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    [HttpGet("scheduleticket/{id}")]
    public async Task<IActionResult> GetListTicketScheduleByTourID(Guid id)
    {
        var response = await mediator.Send(new GetListTicketScheduleByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    [HttpPut("closetour/{id}")]
    public async Task<IActionResult> CloseTour(Guid id)
    {
        var response = await mediator.Send(new CloseTourCommand(id));
        return HandleServiceResult(response);
    }
    [HttpGet("schedule/{id}")]
    public async Task<IActionResult> GetTourScheduleByTourID(Guid id)
    {
        var response = await mediator.Send(new GetTourScheduleByTourIDQuery(id));
        return HandleServiceResult(response);
    }

}