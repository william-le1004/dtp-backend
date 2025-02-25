using Domain.Entities;
using Infrastructure;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DtpDbContext _context;

        public UserController(DtpDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return users;
        }
        
    }
}
