using Application.Features.Tour.Commands;
using Application.Features.Tour.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController(ILogger<TourController> logger, IMediator mediator)
    : BaseController
{
    [HttpGet]
    [EnableQuery]
    public async Task<IQueryable<TourTemplateResponse>> Get()
    {
        return await mediator.Send(new GetTours());
    }

    // GET: api/Tour/5
    [HttpGet("{id}")]
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

    [HttpPut("updatetourinfor")]
    public async Task<IActionResult> PutTour([FromBody] UpdateTourInforCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    // Endpoint GET: /api/tour/getlisttourbycompany?companyId={companyId}
    [HttpGet("getlisttourbycompany")]
    public async Task<IActionResult> GetListTourByCompany([FromQuery] Guid companyId)
    {
        var response = await mediator.Send(new GetListTourByCompanyQuery(companyId));
        return HandleServiceResult(response);
    }

    // Endpoint PUT: /api/tour/updatetourdestination
    [HttpPut("updatetourdestination")]
    public async Task<IActionResult> UpdateTourDestination([FromBody] UpdateTourDestinationCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpDelete("deletetourschedule")]
    public async Task<IActionResult> DeleteTourSchedule([FromBody] DeleteTourScheduleCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPost("addschedule")]
    public async Task<IActionResult> AddSchedule([FromBody] AddScheduleCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPut("updateticketschedule")]
    public async Task<IActionResult> UpdateTicketSchedule([FromBody] UpdateTicketScheduleCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    [HttpGet()]
    public async Task<IActionResult> GetTourInforByTourID([FromQuery] Guid id)
    {
        var response = await mediator.Send(new GetTourInforByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    [HttpGet("tourdestination/")]
    public async Task<IActionResult> GetTourDestinationByTourID([FromQuery] Guid id)
    {
        var response = await mediator.Send(new GetTourDestinationByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    // Endpoint GET: /api/tour/getlistticketschedulebytourid?tourId={tourId}
    [HttpGet("scheduleticket/")]
    public async Task<IActionResult> GetListTicketScheduleByTourID([FromQuery] Guid id)
    {
        var response = await mediator.Send(new GetListTicketScheduleByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    // Endpoint GET: /api/tour/gettourschedulebytourid?tourId={tourId}
    [HttpGet("schedule/")]
    public async Task<IActionResult> GetTourScheduleByTourID([FromQuery] Guid id)
    {
        var response = await mediator.Send(new GetTourScheduleByTourIDQuery(id));
        return HandleServiceResult(response);
    }

}