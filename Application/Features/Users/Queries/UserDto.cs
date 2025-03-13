namespace Application.Features.Users.Queries;

public record UserDto(string Id, string UserName, string Email, string CompanyName, string RoleName, bool IsActive);