using FluentValidation;

namespace Application.Features.Users.Commands.Registration;

public class RegistrationValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty()
            .WithMessage("Email is required and must be a valid email address");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$").WithMessage("A valid Vietnamese phone number is required.");
    }
}