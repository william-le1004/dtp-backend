using Application.Features.Tour.Commands;
using Application.Features.Tour.Queries;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController(DtpDbContext context, ILogger<TourController> logger, IMediator mediator)
    : BaseController
{
    //// GET: api/odata/Tours
    //[HttpGet]
    //[EnableQuery]
    //public async Task<IEnumerable<TourResponse>> GetTours()
    //{
    //    return await mediator.Send(new GetTours());
    //}

    //// GET: api/Tour/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<TourDetailResponse>> GetTour(Guid id)
    //{
    //    var result = await mediator.Send(new GetTourDetail(id));

    //    return result.Match<ActionResult<TourDetailResponse>>(
    //        Some: (value) => Ok(value),
    //        None: () => NotFound($"Tour ({id}) not found."));
    //}

  
   
    [HttpPost]
    public async Task<IActionResult> CreateTour([FromBody] CreateToursCommand command)
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

    [HttpPut]
    public async Task<IActionResult> PutTour([FromBody] PutTourCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

}