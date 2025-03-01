using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AccessTokenResponse>
{
    public string RefreshToken { get; set; }
}