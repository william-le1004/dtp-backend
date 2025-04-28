using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(
    string Name,
    string UserName,
    string Email,
    string Address,
    string RoleName,
    string PhoneNumber,
    Guid CompanyId,
    string ConfirmUrl)
    : IRequest<ApiResponse<bool>>;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName)
            .MaximumLength(100).WithMessage("UserName must not exceed 100 characters")
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Email)
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .NotEmpty().EmailAddress().WithMessage("Email is required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$").WithMessage("A valid Vietnamese phone number is required.");

        RuleFor(x => x.Address)
            .MaximumLength(100).WithMessage("Address must not exceed 100 characters")
            .NotEmpty().WithMessage("Address is required");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company Name is required");
    }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IEventBus eventBus,
    ILogger<CreateUserCommandHandler> logger,
    ICompanyRepository companyRepository,
    IAuthenticationService authenticationService)
    : IRequestHandler<CreateUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var newUser = new User(request.UserName, request.Email, request.Name, request.Address, request.PhoneNumber);
            var result = await userRepository.CreateUserAsync(newUser, request.RoleName, request.CompanyId);

            await eventBus.PublishAsync(new UserCreated(
                request.Name,
                request.Email,
                request.UserName,
                $"{request.UserName}{ApplicationConst.DefaultPassword}",
                await companyRepository.GetNameByIdAsync(request.CompanyId),
                await authenticationService.GenerateConfirmUrl(request.Email, request.ConfirmUrl)
            ), cancellationToken);
            
            logger.LogInformation(
                "Published UserCreated event to queue: Name={Name}, UserName={UserName}, Email={Email}",
                request.Name,
                request.UserName,
                request.Email
            );
            return result
                ? ApiResponse<bool>.SuccessResult(true)
                : ApiResponse<bool>.Failure("User creation failed");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, [ex.Message]);
        }
    }
}