using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Dtos;
using Application.Messaging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record RegistrationCommand(
    string Name,
    string Address,
    string Email,
    string UserName,
    string PhoneNumber,
    string Password,
    string ConfirmUrl)
    : IRequest<ApiResponse<bool>>;

public class RegistrationValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email is required and must be a valid email address");
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
        RuleFor(x => x.ConfirmUrl)
            .NotEmpty()
            .WithMessage("ConfirmUrl is required");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$").WithMessage("A valid Vietnamese phone number is required.");
    }
}

public class RegistrationHandler(
    IAuthenticationService authenticationService,
    IEventBus eventBus,
    ILogger<RegistrationHandler> logger)
    : IRequestHandler<RegistrationCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var validator = new RegistrationValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var registrationRequest = new RegistrationRequestDto(
                request.Name, request.Address, request.Email, request.UserName, request.PhoneNumber, request.Password, request.ConfirmUrl);
            await authenticationService.RegisterAsync(registrationRequest);
            await eventBus.PublishAsync(
                new EmailConfirmed(
                    request.Name,
                    request.Email,
                    await authenticationService.GenerateConfirmUrl(request.Email, request.ConfirmUrl)
                ),
                cancellationToken
            );
            
            logger.LogInformation("User {UserName} with {Email} registered successfully", request.UserName, request.Email);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, [ex.Message]);
        }
    }
}