using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(DtpDbContext context) : BaseController
{
    // GET: api/Category
    [HttpGet]
    [EnableQuery]
    [Authorize(Policy = ApplicationConst.HighLevelPermission)]
    public IQueryable<Category> Get()
    {
        return context.Categories.AsQueryable();
    }

    // GET: api/Category/5
    [HttpGet("{id}")]
    [Authorize(Policy = ApplicationConst.HighLevelPermission)]
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
    [Authorize(Roles = ApplicationRole.ADMIN)]
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
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCate", new { id = category.Id }, category);
    }

    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    [Authorize(Roles = ApplicationRole.ADMIN)]
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