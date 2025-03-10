using FluentValidation;

namespace Application.Features.Users.Commands.Create;

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