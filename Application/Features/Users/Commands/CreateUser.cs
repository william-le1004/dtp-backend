using Application.Common;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Events;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(
    string Name,
    string UserName,
    string Email,
    string Address,
    string RoleName,
    string PhoneNumber,
    Guid CompanyId)
    : IRequest<ApiResponse<bool>>;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().WithMessage("Email is required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$").WithMessage("A valid Vietnamese phone number is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required");
    }
}

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public CreateUserCommandHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

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
            var result = await _userRepository.CreateUserAsync(newUser, request.RoleName, request.CompanyId);
            await _eventBus.PublishAsync(new UserCreated
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                Password = $"{request.UserName}{ApplicationConst.DefaultPassword}"
            }, cancellationToken);
            return result
                ? ApiResponse<bool>.SuccessResult(true)
                : ApiResponse<bool>.Failure("User creation failed");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, new List<string> { ex.Message });
        }
    }
}