using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Login;

public class LoginHandler
    : IRequestHandler<LoginCommand, ApiResponse<AccessTokenResponse>>
{
    private IAuthenticationService _authenticationService;

    public LoginHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<ApiResponse<AccessTokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validator = new LoginCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<AccessTokenResponse>.Failure("Validation failed", 400, errors);
        }
        
        var user = new LoginRequestDto(request.UserName, request.Password);
        var result = await _authenticationService.LoginAsync(user);
        if(!result.Success)
        {
            return ApiResponse<AccessTokenResponse>.Failure(result.Message, 400);
        }
        
        return ApiResponse<AccessTokenResponse>.SuccessResult(result.Data, "User login successfully");
    }
}