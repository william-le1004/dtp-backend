using Application.Features.Company.Queries;

namespace Application.Features.Company.Mapping;

public static class CompanyMappingExtensions
{
    public static CompanyDto MapToCompanyDto(this Domain.Entities.Company entity) =>
        new(
            entity.Id,
            entity.Name,
            entity.Phone,
            entity.Email,
            entity.TaxCode,
            entity.Address,
            entity.Licensed,
            entity.StaffCount(),
            entity.TourCount(),
            entity.CommissionRate,
            entity.IsDeleted
        );

    public static CompanyDetailDto MapToCompanyDetailDto(this Domain.Entities.Company entity,
        Dictionary<string, string> staffRoles) =>
        new(
            entity.Id,
            entity.Name,
            entity.Phone,
            entity.Email,
            entity.TaxCode,
            entity.Address,
            entity.Licensed,
            entity.CommissionRate,
            entity.Staffs.Select(s => new StaffDto(
                s.Id.ToString(),
                s.Name,
                s.PhoneNumber ?? "N/A",
                s.Email ?? "N/A",
                staffRoles.GetValueOrDefault(s.Id) ?? "N/A"
            )).ToList(),
            entity.Tours.Select(t => new Tours(t.Id, t.Title)).ToList()
        );
}