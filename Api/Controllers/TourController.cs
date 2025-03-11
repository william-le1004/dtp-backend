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
    // GET: api/odata/Tours
    [HttpGet]
    [EnableQuery]
    public async Task<IEnumerable<TourTemplateResponse>> GetTours()
    {
        return await mediator.Send(new GetTours());
    }

    // GET: api/Tour/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TourDetailResponse>> GetTour(Guid id)
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

    // Endpoint PUT: /api/tour/updateticketschedule
    [HttpPut("updateticketschedule")]
    public async Task<IActionResult> UpdateTicketSchedule([FromBody] UpdateTicketScheduleCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    [HttpGet("gettourinforbytourid")]
    public async Task<IActionResult> GetTourInforByTourID([FromQuery] Guid tourId)
    {
        var response = await mediator.Send(new GetTourInforByTourIDQuery(tourId));
        return HandleServiceResult(response);
    }
    [HttpGet("gettourdestinationbytourid")]
    public async Task<IActionResult> GetTourDestinationByTourID([FromQuery] Guid tourId)
    {
        var response = await mediator.Send(new GetTourDestinationByTourIDQuery(tourId));
        return HandleServiceResult(response);
    }
    // Endpoint GET: /api/tour/getlistticketschedulebytourid?tourId={tourId}
    [HttpGet("getlistticketschedulebytourid")]
    public async Task<IActionResult> GetListTicketScheduleByTourID([FromQuery] Guid tourId)
    {
        var response = await mediator.Send(new GetListTicketScheduleByTourIDQuery(tourId));
        return HandleServiceResult(response);
    }
    // Endpoint GET: /api/tour/gettourschedulebytourid?tourId={tourId}
    [HttpGet("gettourschedulebytourid")]
    public async Task<IActionResult> GetTourScheduleByTourID([FromQuery] Guid tourId)
    {
        var response = await mediator.Send(new GetTourScheduleByTourIDQuery(tourId));
        return HandleServiceResult(response);
    }

}