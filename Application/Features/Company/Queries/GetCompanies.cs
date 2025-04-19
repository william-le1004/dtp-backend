using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Company.Mapping;
using MediatR;

namespace Application.Features.Company.Queries;

public record GetCompaniesQuery : IRequest<ApiResponse<IQueryable<CompanyDto>>>;

public class GetCompaniesQueryHandler(ICompanyRepository companyRepository)
    : IRequestHandler<GetCompaniesQuery, ApiResponse<IQueryable<CompanyDto>>>
{
    public async Task<ApiResponse<IQueryable<CompanyDto>>> Handle(GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await companyRepository.GetCompaniesAsync();
        var companyDtos = companies.Select(x => x.MapToCompanyDto()).ToList();
        
        return ApiResponse<IQueryable<CompanyDto>>.SuccessResult(companyDtos.AsQueryable());
    }
}