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

    // PUT: api/Tour/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    // [HttpPut("{id}")]
    // public async Task<IActionResult> PutTour(Guid id, Tour tour)
    // {
    //     if (id != tour.Id)
    //     {
    //         return BadRequest();
    //     }
    //
    //     context.Entry(tour).State = EntityState.Modified;
    //
    //     try
    //     {
    //         await context.SaveChangesAsync();
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         if (!TourExists(id))
    //         {
    //             return NotFound();
    //         }
    //         else
    //         {
    //             throw;
    //         }
    //     }
    //
    //     return NoContent();
    // }
    //
    // // POST: api/Tour
    // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    // //[HttpPost]
    // //public async Task<ActionResult<Tour>> PostTour(Tour tour)
    // //{
    // //    context.Tours.Add(tour);
    // //    await context.SaveChangesAsync();
    //
    // //    return CreatedAtAction("GetTour", new { id = tour.Id }, tour);
    // //}
    //
    // // DELETE: api/Tour/5
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteTour(Guid id)
    // {
    //     var tour = await context.Tours.FindAsync(id);
    //     if (tour == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     context.Tours.Remove(tour);
    //     await context.SaveChangesAsync();
    //
    //     return NoContent();
    // }
    //
    // private bool TourExists(Guid id)
    // {
    //     return context.Tours.Any(e => e.Id == id);
    // }

    [HttpPost("create")]
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

    [HttpPut("update")]
    public async Task<IActionResult> PutTour([FromBody] UpdateTourCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPost("create-destination")]
    public async Task<IActionResult> AddDestinationsToTour([FromBody] CreateDestinationsCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
}