using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ApiResponse<AccessTokenResponse>>;

public class RefreshTokenCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<AccessTokenResponse>>
{
    public async Task<ApiResponse<AccessTokenResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var newToken = await authenticationService.RefreshTokenAsync(request.RefreshToken);
            return ApiResponse<AccessTokenResponse>.SuccessResult(newToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<AccessTokenResponse>.Failure($"An error occurred", 400, new List<string> { ex.Message });
        }
    }
}