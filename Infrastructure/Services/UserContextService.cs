using System.Security.Claims;
using Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public string? GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public List<string> GetCurrentUserRoles()
    {
        return httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
    }

    public bool IsAdminRole()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true &&
               user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    public bool IsOperatorRole()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true &&
               user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Operator");
    }

    public string? GetAccessToken()
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        return authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring(7) : null;
    }

    public Guid? GetCompanyId()
    {
        var companyId = httpContextAccessor.HttpContext?.User?.FindFirstValue("company_id");
        return companyId != null ? Guid.Parse(companyId) : (Guid?)null;
    }
}