using Application.Features.Order.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Contexts;
using MediatR;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OderController(DtpDbContext context, IMediator mediator) : ControllerBase
{
    // GET: api/Oder
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TourBooking>>> GetTourBookings()
    {
        return await context.TourBookings.ToListAsync();
    }

    // GET: api/Oder/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TourBooking>> GetTourBooking(Guid id)
    {
        var tourBooking = await context.TourBookings.FindAsync(id);

        if (tourBooking == null)
        {
            return NotFound();
        }

        return tourBooking;
    }


    // POST: api/Oder
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TourBooking>> Checkout(PlaceOrderCommand command)
    {
        var order = await mediator.Send(command);

        return order;
    }

    // DELETE: api/Oder/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTourBooking(Guid id)
    {
        var tourBooking = await context.TourBookings.FindAsync(id);
        if (tourBooking == null)
        {
            return NotFound();
        }

        context.TourBookings.Remove(tourBooking);
        await context.SaveChangesAsync();

        return NoContent();
    }
}