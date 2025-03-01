using FluentValidation;

namespace Application.Features.Users.Commands.Registration;

public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
}