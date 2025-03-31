using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Company.Queries;

public record GetCompaniesQuery : IRequest<ApiResponse<IQueryable<CompanyDto>>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, ApiResponse<IQueryable<CompanyDto>>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<IQueryable<CompanyDto>>> Handle(GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetCompaniesAsync();

        return ApiResponse<IQueryable<CompanyDto>>.SuccessResult(companies.Select(c => new CompanyDto(
            c.Id,
            c.Name,
            c.Phone,
            c.Email,
            c.TaxCode,
            c.Licensed,
            c.StaffCount(),
            c.TourCount(),
            c.CommissionRate
        )).AsQueryable());
    }
}