using Application.Common;
using Application.Contracts.Persistence;
using FluentValidation;
using MediatR;

namespace Application.Features.Company.Commands;

public record CreateCompanyCommand(string Name, string Email, string Phone, string TaxCode, string UserId)
    : IRequest<ApiResponse<bool>>;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$").WithMessage("A valid Vietnamese phone number is required.");

        RuleFor(x => x.TaxCode)
            .NotEmpty().WithMessage("Tax Code is required.")
            .MaximumLength(20).WithMessage("Tax Code must not exceed 20 characters.");
    }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, ApiResponse<bool>>
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<bool>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateCompanyValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var newCompany = new Domain.Entities.Company(request.Name, request.Email, request.Phone, request.TaxCode);
  
            await _companyRepository.UpsertCompanyAsync(newCompany, request.UserId);

            return ApiResponse<bool>.SuccessResult(true, "Company created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, new List<string> { ex.Message });
        }
    }
}