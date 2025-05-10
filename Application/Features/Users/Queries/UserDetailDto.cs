namespace Application.Features.Users.Queries;

public record UserDetailDto(
    string Id,
    string? UserName,
    decimal Balance,
    string? Name,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? CompanyName,
    Guid? CompanyId,
    string RoleName,
    bool IsActive
);