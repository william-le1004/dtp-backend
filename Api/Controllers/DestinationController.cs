using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DestinationController(DtpDbContext context) : ODataController
{
    // GET: api/Destination
    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations()
    {
        return await context.Destinations.ToListAsync();
    }

    // GET: api/Destination/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Destination>> GetDestination(Guid id)
    {
        var destination = await context.Destinations.FindAsync(id);

        if (destination == null)
        {
            return NotFound();
        }

        return destination;
    }

    // PUT: api/Destination/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDestination(Guid id, Destination destination)
    {
        if (id != destination.Id)
        {
            return BadRequest();
        }

        context.Entry(destination).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DestinationExists(id))
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

    // POST: api/Destination
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Destination>> PostDestination(Destination destination)
    {
        context.Destinations.Add(destination);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetDestination", new { id = destination.Id }, destination);
    }

    // DELETE: api/Destination/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestination(Guid id)
    {
        var destination = await context.Destinations.FindAsync(id);
        if (destination == null)
        {
            return NotFound();
        }

        destination.IsDeleted = true;
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool DestinationExists(Guid id)
    {
        return context.Destinations.Any(e => e.Id == id);
    }
}