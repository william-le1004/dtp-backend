using System.Net;
using Application.Common;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Company.Commands;

public record DeleteCompanyCommand(Guid Id) : IRequest<ApiResponse<bool>>;

public class DeleteCompanyCommandHandler(ICompanyRepository companyRepository, IRedisCacheService redisCache)
    : IRequestHandler<DeleteCompanyCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        const string cacheKey = "GetAllCompanies";
        var company = await companyRepository.GetCompanyAsync(request.Id);

        if (company is null)
            return ApiResponse<bool>.Failure("Company not found");

        try
        {
            company.Delete();
            await companyRepository.UpsertCompanyAsync(company);
            await redisCache.RemoveDataAsync(cacheKey);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", (int)HttpStatusCode.BadRequest, new List<string> { ex.Message });
        }
    }
}