using Application.Common;
using Application.Contracts.Persistence;
using FluentValidation;
using MediatR;

namespace Application.Features.Company.Commands;

public record UpdateCompanyCommand(Guid Id, string Name, string Email, string Phone, string TaxCode, double CommissionRate)
    : IRequest<ApiResponse<bool>>;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
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

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ApiResponse<bool>>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateCompanyValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        var company = await _companyRepository.GetCompanyAsync(request.Id);

        if (company == null)
        {
            return ApiResponse<bool>.Failure("Company not found");
        }

        try
        {
            company.UpdateDetails(request.Name, request.Email, request.Phone, request.TaxCode, request.CommissionRate);
            await _companyRepository.UpsertCompanyAsync(company);
            return ApiResponse<bool>.SuccessResult(true, "Company updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", 400, new List<string> { ex.Message });
        }
    }
}