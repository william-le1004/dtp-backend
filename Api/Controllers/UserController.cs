// using Domain.Entities;
// using Infrastructure;
// using Infrastructure.Contexts;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace Api.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class UserController(IMediator _mediator) : ControllerBase
//     {
//         [HttpGet]
//         public async Task<IEnumerable<Users>> Get()
//         {
//             var users = await _mediator.Send();
//             return users;
//         }
//         
//     }
// }
