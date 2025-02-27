using Application.Tour.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController(DtpDbContext context, ILogger<TourController> logger, IMediator mediator)
    : ODataController
{
    // GET: api/odata/Tours
    [HttpGet]
    [EnableQuery]
    public async Task<IEnumerable<TourResponse>> GetTours()
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
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTour(Guid id, Tour tour)
    {
        if (id != tour.Id)
        {
            return BadRequest();
        }

        context.Entry(tour).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TourExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Tour
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Tour>> PostTour(Tour tour)
    {
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetTour", new { id = tour.Id }, tour);
    }

    // DELETE: api/Tour/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTour(Guid id)
    {
        var tour = await context.Tours.FindAsync(id);
        if (tour == null)
        {
            return NotFound();
        }

        context.Tours.Remove(tour);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool TourExists(Guid id)
    {
        return context.Tours.Any(e => e.Id == id);
    }
}