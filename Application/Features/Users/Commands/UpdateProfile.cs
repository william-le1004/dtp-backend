using Application.Common;
using Application.Contracts.Persistence;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands;

public record UpdateProfileCommand(
    string Id,
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    string RoleName
) : IRequest<ApiResponse<bool>>;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
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
    }
}

public class UpdateProfileCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateProfileCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var user = await userRepository.GetUserDetailAsync(request.Id);
            if (user is null) return ApiResponse<bool>.Failure("User not found", 404);

            user.UpdateProfile(request.Name, request.Address, request.PhoneNumber, request.Email, request.UserName);

            var result = await userRepository.UpdateProfileAsync(user, request.RoleName);
            return result ? ApiResponse<bool>.SuccessResult(true) : ApiResponse<bool>.Failure("Profile update failed");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, [ex.Message]);
        }
    }
}