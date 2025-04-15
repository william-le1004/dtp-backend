using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Messaging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record ForgetPasswordCommand(
    string Email,
    string ConfirmUrl)
    : IRequest<ApiResponse<bool>>;

public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email is required and must be a valid email address");
    }
}

public class ForgetPasswordHandler(
    IAuthenticationService authenticationService,
    IEventBus eventBus,
    ILogger<ForgetPasswordHandler> logger)
    : IRequestHandler<ForgetPasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validator = new ForgetPasswordValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            await eventBus.PublishAsync(
                new PasswordForget(
                    request.Email,
                    await authenticationService.GenerateConfirmUrl(request.Email, request.ConfirmUrl)
                ),
                cancellationToken
            );

            logger.LogInformation("Reset Password Gmail already send to {Email}", request.Email);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, [ex.Message]);
        }
    }
}