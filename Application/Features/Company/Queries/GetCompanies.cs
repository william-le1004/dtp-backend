using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Company.Queries;

public record GetCompaniesQuery : IRequest<ApiResponse<List<CompanyDto>>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, ApiResponse<List<CompanyDto>>>
{
    private readonly IDtpDbContext _context;

    public GetCompaniesQueryHandler(IDtpDbContext context, UserManager<User> userManager)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<CompanyDto>>> Handle(GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        
        var companies = await _context.Companies.Include(x => x.Staffs).ToListAsync(cancellationToken);
        return ApiResponse<List<CompanyDto>>.SuccessResult(companies.Select(c => new CompanyDto(
            c.Id,
            c.Name,
            c.Phone,
            c.Email,
            c.TaxCode,
            c.Licensed,
            c.Staffs.Select(x => new StaffDto(x.Id, x.Name, x.PhoneNumber, x.Email)).FirstOrDefault()
        )).ToList());
    }
}