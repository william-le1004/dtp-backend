using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = ApplicationRole.ADMIN)]
public class CategoryController(DtpDbContext context) : BaseController
{
    // GET: api/Category
    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        return await context.Categories.ToListAsync();
    }

    // GET: api/Category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCate(Guid id)
    {
        var category = await context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }

    // PUT: api/Category/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(Guid id, Category category)
    {
        var exitedCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (exitedCategory == null)
        {
            return NotFound();
        }

        exitedCategory.Name = category.Name;
        context.Categories.Update(exitedCategory);
        await context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Category
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCate", new { id = category.Id }, category);
    }

    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.IsDeleted = true;
        await context.SaveChangesAsync();

        return NoContent();
    }
}