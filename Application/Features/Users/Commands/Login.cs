using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands;

public record LoginCommand(string UserName, string Password, string FcmToken = "")
    : IRequest<ApiResponse<AccessTokenResponse>>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}

public class LoginHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, ApiResponse<AccessTokenResponse>>
{
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
            var tokenResponse = await authenticationService.LoginAsync(user);

            return ApiResponse<AccessTokenResponse>.SuccessResult(tokenResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<AccessTokenResponse>.Failure($"An error occurred", 400, [ex.Message]);
        }
    }
}