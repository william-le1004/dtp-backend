namespace Application.Contracts;

public interface IUserContextService
{
    string? GetCurrentUserId();
    List<string> GetCurrentUserRoles();
    bool IsAdminRole();
    bool IsOperatorRole();
    string? GetAccessToken();
    Guid? GetCompanyId();
}