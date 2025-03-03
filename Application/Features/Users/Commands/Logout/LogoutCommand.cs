using Application.Common;
using MediatR;

namespace Application.Features.Users.Commands.Logout;

public class LogoutCommand : IRequest<ApiResponse<bool>>
{
    public string UserId { get; set; }

    public LogoutCommand(string userId)
    {
        UserId = userId;
    }
}