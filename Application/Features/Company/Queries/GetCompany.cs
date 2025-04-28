using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Company.Mapping;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Company.Queries;

public record GetCompanyQuery(Guid Id) : IRequest<ApiResponse<CompanyDetailDto>>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, ApiResponse<CompanyDetailDto>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<User> _userManager;

    public GetCompanyQueryHandler(ICompanyRepository companyRepository, UserManager<User> userManager)
    {
        _companyRepository = companyRepository;
        _userManager = userManager;
    }

    public async Task<ApiResponse<CompanyDetailDto>> Handle(GetCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _companyRepository.GetCompanyAsync(request.Id);

        if (result == null)
        {
            return ApiResponse<CompanyDetailDto>.Failure("Company not found");
        }

        var staffRoles = new Dictionary<string, string>();
        foreach (var staff in result.Staffs)
        {
            var roles = await _userManager.GetRolesAsync(staff);
            staffRoles[staff.Id] = roles.FirstOrDefault();
        }

        return ApiResponse<CompanyDetailDto>.SuccessResult(result.MapToCompanyDetailDto(staffRoles));
    }
}