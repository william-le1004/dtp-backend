using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = ApplicationConst.HighLevelPermission)]
public class DestinationController(DtpDbContext context) : BaseController
{
    // GET: api/Destination
    [HttpGet]
    [EnableQuery]
    public IQueryable<Destination> Get()
    {
        return context.Destinations.AsQueryable();
    }

    // GET: api/Destination/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Destination>> GetDest(Guid id)
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
        var exitedDest = await context.Destinations.FirstOrDefaultAsync(x => x.Id == id);

        if (exitedDest == null)
        {
            return NotFound();
        }

        exitedDest.Name = destination.Name;
        exitedDest.Longitude = destination.Longitude;
        exitedDest.Latitude = destination.Latitude;
        context.Destinations.Update(exitedDest);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Destination
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Destination>> PostDestination(Destination destination)
    {
        context.Destinations.Add(destination);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDest), new { id = destination.Id }, destination);
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