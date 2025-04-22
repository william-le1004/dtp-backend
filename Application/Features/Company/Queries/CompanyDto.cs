namespace Application.Features.Company.Queries;

public record CompanyDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    string Address,
    bool Licensed,
    int Staff,
    int TourCount,
    double CommissionRate,
    bool IsDelete);

public record CompanyDetailDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    string Address,
    bool Licensed,
    double CommissionRate,
    List<StaffDto> Staffs,
    List<Tours> Tours);

public record Tours(Guid Id, string Title);

public record StaffDto(
    string Id,
    string Name,
    string Phone,
    string Email,
    string RoleName);