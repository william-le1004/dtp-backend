using Application.Features.AdminAnalysis.Queries;
using Application.Features.Feedback.Commands;
using Application.Features.OperatorAnalysis.Queries;
using Application.Features.Rating.Commands;
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
    public async Task<IActionResult> PutTour(Guid id, [FromBody] UpdateTourInforCommand command)
    {
        var updatedCommand = command with { TourId = id };
        var response = await mediator.Send(updatedCommand);
        return HandleServiceResult(response);

    }
    [HttpGet("company")]
    public async Task<IActionResult> GetListTourByCompanyID()
    {
        var response = await mediator.Send(new GetListTourByCompanyQuery());
        return HandleServiceResult(response);
    }

    [HttpPut("tourdestination/{id}")]
    public async Task<IActionResult> UpdateTourDestination(Guid id, [FromBody] UpdateTourDestinationCommand command)
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

    [HttpPut("scheduleticket/{id}")]
    public async Task<IActionResult> UpdateTicketSchedule(Guid id, [FromBody] UpdateTicketScheduleByTicketKindCommand command)
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
    public async Task<IActionResult> CloseTour(Guid id, [FromBody] string remark)
    {    
        var response = await mediator.Send(new CloseTourCommand(id,remark));
        return HandleServiceResult(response);
    }
    [HttpGet("schedule/{id}")]
    public async Task<IActionResult> GetTourScheduleByTourID(Guid id)
    {
        var response = await mediator.Send(new GetTourScheduleByTourIDQuery(id));
        return HandleServiceResult(response);
    }
    [HttpPost("feedback")]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    [HttpGet("feedback/{id}")]
    public async Task<IActionResult> GetFeedbackByTourID(Guid id)
    {
        var response = await mediator.Send(new GetListFeedbackByTourQuery(id));
        return HandleServiceResult(response);
    }
    [HttpPost("rating")]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    [HttpGet("rating/{id}")]
    public async Task<IActionResult> GetRatingByTourID(Guid id)
    {
        var response = await mediator.Send(new GetListRatingByTourQuery(id));
        return HandleServiceResult(response);
    }
    [HttpGet("operator/analys")]
    public async Task<IActionResult> GetOperatorIncomeAnalysis()
    {
        var response = await mediator.Send(new GetOperatorIncomeAnalysisQuery());
        return HandleServiceResult(response);
    }
    [HttpGet("admin/analys")]
    public async Task<IActionResult> GetAdminIncomeAnalysis()
    {
        var response = await mediator.Send(new GetAdminIncomeAnalysisQuery());
        return HandleServiceResult(response);
    }

}