using Application.Common;
using Application.Contracts.Persistence;
using FluentValidation;
using MediatR;

namespace Application.Features.Company.Commands;

public record GrantLicenseCommand(Guid CompanyId, bool Accept = true)
    : IRequest<ApiResponse<bool>>;


public class GrantLicenseCommandHandler : IRequestHandler<GrantLicenseCommand, ApiResponse<bool>>
{
    private readonly ICompanyRepository _companyRepository;

    public GrantLicenseCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<bool>> Handle(GrantLicenseCommand request, CancellationToken cancellationToken)
    {
        if (!request.Accept)
        {
            return ApiResponse<bool>.SuccessResult(false, "Company Lack Information To Accept License");
        }

        try
        {
            await _companyRepository.GrantCompanyAsync(request.CompanyId);
            return ApiResponse<bool>.SuccessResult(true, "Company Already Granted License");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred: {ex.Message}");
        }
    }
}