using Application.Common;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Company.Mapping;
using MediatR;

namespace Application.Features.Company.Queries;

public record GetCompaniesQuery : IRequest<ApiResponse<IQueryable<CompanyDto>>>;

public class GetCompaniesQueryHandler(ICompanyRepository companyRepository, IRedisCacheService redisCache)
    : IRequestHandler<GetCompaniesQuery, ApiResponse<IQueryable<CompanyDto>>>
{
    public async Task<ApiResponse<IQueryable<CompanyDto>>> Handle(GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "GetAllCompanies";
        var cachedCompanyList = await redisCache.GetDataAsync<List<CompanyDto>>(cacheKey);
        if (cachedCompanyList != null)
        {
            return ApiResponse<IQueryable<CompanyDto>>.SuccessResult(cachedCompanyList.AsQueryable());
        }
        
        var companies = await companyRepository.GetCompaniesAsync();
        var companyDtos = companies.Select(x => x.MapToCompanyDto()).ToList();
        
        await redisCache.SetDataAsync(cacheKey, companyDtos, TimeSpan.FromMinutes(10));
        return ApiResponse<IQueryable<CompanyDto>>.SuccessResult(companyDtos.AsQueryable());
    }
}