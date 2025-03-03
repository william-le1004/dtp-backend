using FluentValidation;

namespace Application.Features.Users.Commands.Registration;

public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty()
            .WithMessage("Email is required and must be a valid email address");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        // RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Name is required");
    }
}