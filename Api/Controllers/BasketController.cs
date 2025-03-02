using Application.Features.Basket.Commands;
using Application.Features.Basket.Queries;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using MediatR;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController(IMediator mediator) : ControllerBase
{
    // GET: api/Basket
    [HttpGet]
    public async Task<IEnumerable<BasketTourItemResponse>> GetBaskets()
        => await mediator.Send(new GetBaskets());

    // PUT: api/Basket/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<IActionResult> PostBasket(AddTourToBasket request)
    {
        await mediator.Send(request);
        return NoContent();
    }

    // POST: api/Basket
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    public async Task<ActionResult<Basket>> PutBasket(Basket basket)
    {
        return CreatedAtAction("GetBasket", new { id = basket.Id }, basket);
    }

    // DELETE: api/Basket/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBasket(Guid id)
    {
        await mediator.Send(new RemoveTourFromBasket(id));
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBasket()
    {
        await mediator.Send(new RemoveAllFromBasket());
        return NoContent();
    }
}