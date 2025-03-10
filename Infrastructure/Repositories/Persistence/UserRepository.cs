using Application.Contracts;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Users.Queries.Get;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Persistence;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly DtpDbContext _dtpDbContext;
    private readonly IUserContextService _userContextService;
    private readonly IRedisCacheService _redisCache;

    public UserRepository(DtpDbContext dtpDbContext, UserManager<User> userManager,
        IUserContextService userContextService, IRedisCacheService redisCache)
    {
        _dtpDbContext = dtpDbContext;
        _userManager = userManager;
        _userContextService = userContextService;
        _redisCache = redisCache;
    }

    public async Task<bool> InactiveUser(User user)
    {
        if (_userContextService.IsAdminRole())
        {
            await _dtpDbContext.SaveChangesAsync();
            return true;
        }

        if (_userContextService.IsOperatorRole())
        {
            var isManager = await _userManager.IsInRoleAsync(user, ApplicationRole.MANAGER);
            if (isManager)
            {
                await _dtpDbContext.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        IQueryable<User> query = _dtpDbContext.Users
            .Include(x => x.Company)
            .Include(x => x.Wallet);
        
        if (!_userContextService.IsAdminRole())
        {
            var managerCompanyIds = (await _userManager.GetUsersInRoleAsync(ApplicationRole.MANAGER))
                .Select(m => m.CompanyId)
                .ToList();

            query = query.Where(x => managerCompanyIds.Contains(x.CompanyId));
        }

        return await query.OrderBy(x => x.CreatedAt).ToListAsync();
    }

    public async Task<User?> GetUserDetailAsync(string userId)
    {
        IQueryable<User> query = _dtpDbContext.Users.Where(x => x.Id == userId)
            .Include(x => x.Company)
            .Include(x => x.Wallet);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> CreateUser(User user, string role)
    {
        var result = await _userManager.CreateAsync(user, $"{user.UserName}{ApplicationConst.DEFAULT_PASSWORD}");
        if (!result.Succeeded) return false;

        await AssignRole(user, role);
        return await SaveChangesIfNeededAsync();
    }

    public async Task<bool> UpdateProfile(User user, string role)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;

        if (!string.IsNullOrEmpty(role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            await AssignRole(user, role);
        }

        return await SaveChangesIfNeededAsync();
    }

    private async Task AssignRole(User user, string role)
    {
        var currentRole = _userContextService.GetCurrentUserRoles();

        if (currentRole.Contains(ApplicationRole.ADMIN) || currentRole.Contains(ApplicationRole.OPERATOR))
        {
            var roleToAssign = currentRole.Contains(ApplicationRole.ADMIN) ? role : ApplicationRole.MANAGER;
            await _userManager.AddToRoleAsync(user, roleToAssign);
        }
    }

    private async Task<bool> SaveChangesIfNeededAsync()
    {
        await _dtpDbContext.SaveChangesAsync();
        return true;
    }
}