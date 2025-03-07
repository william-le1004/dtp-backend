using Application.Common;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ApiResponse<AccessTokenResponse>>;