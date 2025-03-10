using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Company.Queries;

public record GetCompanyQuery(Guid Id) : IRequest<ApiResponse<CompanyDetailDto>>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, ApiResponse<CompanyDetailDto>>
{
    private readonly IDtpDbContext _context;
    private readonly UserManager<User> _userManager;

    public GetCompanyQueryHandler(IDtpDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApiResponse<CompanyDetailDto>> Handle(GetCompanyQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _context.Companies
            .Include(c => c.Staffs)
            .Include(c => c.Tours)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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

        var staffDto = result.Staffs
            .Select(staff => new StaffDto(
                staff.Id,
                staff.Name,
                staff.PhoneNumber,
                staff.Email,
                staffRoles.GetValueOrDefault(staff.Id)
            ))
            .ToList();

        var tourDto = result.Tours
            .Select(t => new Tours(t.Id, t.Title))
            .ToList();

        return ApiResponse<CompanyDetailDto>.SuccessResult(new CompanyDetailDto(
            result.Id,
            result.Name,
            result.Phone,
            result.Email,
            result.TaxCode,
            result.Licensed,
            staffDto.ToList(),
            tourDto.ToList()
        ));
    }
}