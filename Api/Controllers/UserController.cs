using System.Security.Claims;
using Application.Common;
using Application.Contracts;
using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.Queries.Get;
using Application.Features.Users.Queries.GetDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AuthUsers")]
public class UserController: BaseController
{
    private readonly IMediator _mediator;
    private readonly IUserContextService _userContextService;

    public UserController(IMediator mediator, IUserContextService userContextService)
    {
        _mediator = mediator;
        _userContextService = userContextService;
    }
    
    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        var userId = _userContextService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Failure("User not authenticated", 401));
        }
        var response = await _mediator.Send(new GetUserDetailQuery(userId));

        return HandleServiceResult(response);
    }
    
    [HttpGet("get-all")]
    [Authorize(Policy = "AdminOnly")]
    [EnableQuery]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetUserQuery());
        return HandleServiceResult(response);
    }
    
    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand createUserCommand)
    {
        var response = await _mediator.Send(createUserCommand);
        return HandleServiceResult(response);
    }
    
    [HttpPut("delete{userId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(string userId)
    {
        var response = await _mediator.Send(new DeleteUserCommand(userId));
        return HandleServiceResult(response);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand profileCommand)
    {
        var response = await _mediator.Send(profileCommand);
        return HandleServiceResult(response);
    }
}
