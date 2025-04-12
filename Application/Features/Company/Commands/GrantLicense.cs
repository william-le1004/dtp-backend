using System.Net;
using Application.Common;
using Application.Contracts.Persistence;
using FluentValidation;
using MediatR;

namespace Application.Features.Company.Commands;

public record GrantLicenseCommand(Guid CompanyId, bool Accept = true)
    : IRequest<ApiResponse<bool>>;


public class GrantLicenseCommandHandler(ICompanyRepository companyRepository)
    : IRequestHandler<GrantLicenseCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(GrantLicenseCommand request, CancellationToken cancellationToken)
    {
        if (!request.Accept)
        {
            return ApiResponse<bool>.SuccessResult(false, "Company Lack Information To Accept License");
        }

        try
        {
            await companyRepository.GrantCompanyAsync(request.CompanyId);
            return ApiResponse<bool>.SuccessResult(true, "Company Already Granted License");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", (int)HttpStatusCode.BadRequest, new List<string> { ex.Message });
        }
    }
}