namespace Application.Features.Company.Queries;

public record CompanyDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    bool Lisenced,
    StaffDto Staff);

public record CompanyDetailDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    bool Lisenced,
    List<StaffDto> Staffs);

public record StaffDto(string Id, string Name, string Phone, string Email, string RoleName = "");