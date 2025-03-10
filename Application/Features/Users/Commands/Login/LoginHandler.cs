using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Login;

public class LoginHandler
    : IRequestHandler<LoginCommand, ApiResponse<AccessTokenResponse>>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<ApiResponse<AccessTokenResponse>> Handle(LoginCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new LoginValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<AccessTokenResponse>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var user = new LoginRequestDto(request.UserName, request.Password);
            var tokenResponse = await _authenticationService.LoginAsync(user);

            return ApiResponse<AccessTokenResponse>.SuccessResult(tokenResponse, "User login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AccessTokenResponse>.Failure(ex.Message, 400);
        }
    }
}