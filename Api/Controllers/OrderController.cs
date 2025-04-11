using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController(IMediator mediator) : ControllerBase
{
    // GET: api/Oder
    [HttpGet]
    public async Task<IEnumerable<OrderResponses>> GetTourBookings()
    {
        return await mediator.Send(new GetOrders());
    }

    // GET: api/Oder/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailResponse>> GetTourBooking(Guid id)
    {
        var result = await mediator.Send(new GetOrderDetail(id));

        return result.Match<ActionResult<OrderDetailResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Order ({id}) not found."));
    }

    // POST: api/Oder
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TourBooking>> Checkout(PlaceOrderCommand command)
    {
        var order = await mediator.Send(command);
        return CreatedAtAction(nameof(GetTourBookings), new { id = order.Id });
    }

    // DELETE: api/Oder/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Guid>> CancelBooking(Guid id,[FromBody] string? remake)
    {
        var order = await mediator.Send(new CancelOrder(id, remake));

        return order.Match<ActionResult<Guid>>(
            Some: (value) => NoContent(),
            None: () => BadRequest($"Order ({id}) not found."));
    }
}