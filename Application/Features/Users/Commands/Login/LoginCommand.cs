using Application.Common;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Login;

public record LoginCommand(string UserName, string Password)
    : IRequest<ApiResponse<AccessTokenResponse>>;