using Application.Common;
using Application.Contracts;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = ApplicationConst.AuthenticatedUser)]
public class UserController(IMediator mediator, IUserContextService userContextService) : BaseController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetById()
    {
        var userId = userContextService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Failure("User not authenticated", 401));
        }

        var response = await mediator.Send(new GetUserDetailQuery(userId));

        return HandleServiceResult(response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var response = await mediator.Send(new GetUserDetailQuery(id));
        return HandleServiceResult(response);
    }

    [HttpGet("all")]
    [Authorize(Policy = ApplicationConst.ManagementPermission)]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var response = await mediator.Send(new GetUserQuery());
        return ReturnList(response);
    }

    [HttpPost]
    [Authorize(Policy = ApplicationConst.ManagementPermission)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand createUserCommand)
    {
        var response = await mediator.Send(createUserCommand);
        return HandleServiceResult(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Inactive([FromRoute] string id)
    {
        var response = await mediator.Send(new DeleteUserCommand(id));
        return HandleServiceResult(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand profileCommand)
    {
        var response = await mediator.Send(profileCommand);
        return HandleServiceResult(response);
    }
    
    [HttpPut("password")]
    public async Task<IActionResult> ChangPassword([FromBody] ChangePasswordCommand changePasswordCommand)
    {
        var response = await mediator.Send(changePasswordCommand);
        return HandleServiceResult(response);
    }
}