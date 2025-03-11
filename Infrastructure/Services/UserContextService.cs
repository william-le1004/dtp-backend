using System.Security.Claims;
using Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public List<string> GetCurrentUserRoles()
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
    }

    public bool IsAdminRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            return false;
        }

        return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    public bool IsOperatorRole()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            return false;
        }

        return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Operator");
    }

    public string? GetAccessToken()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        return authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring(7) : null;
    }

    public Guid? GetCompanyId()
    {
        var companyId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("company_id");
        return companyId != null ? Guid.Parse(companyId) : (Guid?)null;
    }
}