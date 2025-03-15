namespace Application.Features.Company.Queries;

public record CompanyDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    bool Licensed,
    int Staff,
    int TourCount);

public record CompanyDetailDto(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string TaxCode,
    bool Licensed,
    List<StaffDto> Staffs,
    List<Tours> Tours);

public record Tours(Guid Id, string Title);

public record StaffDto(
    string Id,
    string Name,
    string Phone,
    string Email,
    string RoleName);