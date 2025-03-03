using Application.Common;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ApiResponse<AccessTokenResponse>>
{
    public string RefreshToken { get; set; }
}