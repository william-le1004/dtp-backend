using Application.Common;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Company.Queries;

public record GetCompanyQuery(Guid Id) : IRequest<ApiResponse<CompanyDetailDto>>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, ApiResponse<CompanyDetailDto>>
{
    private readonly IDtpDbContext _context;

    public GetCompanyQueryHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<CompanyDetailDto>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Companies.FindAsync(request.Id);
        var staffs = await _context.Users.Where(x => x.CompanyId == request.Id).ToListAsync();

        if (result == null)
        {
            return ApiResponse<CompanyDetailDto>.Failure("Company not found");
        }

        return ApiResponse<CompanyDetailDto>.SuccessResult(new CompanyDetailDto(
            result.Id,
            result.Name,
            result.Phone,
            result.Email,
            result.TaxCode,
            result.Licensed,
            result.Staffs.Select(x => new StaffDto(x.Id, x.Name, x.PhoneNumber, x.Email)).ToList()
        ));
    }
}