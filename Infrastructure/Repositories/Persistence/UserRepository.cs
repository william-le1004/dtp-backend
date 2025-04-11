using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Constants;
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

    public UserRepository(DtpDbContext dtpDbContext, UserManager<User> userManager,
        IUserContextService userContextService)
    {
        _dtpDbContext = dtpDbContext;
        _userManager = userManager;
        _userContextService = userContextService;
    }

    public async Task<bool> InactiveUserAsync(User user)
    {
        user.Deactivate();
        return await SaveChangesIfNeededAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        IQueryable<User> query = _dtpDbContext.Users
            .AsSplitQuery()
            .Include(x => x.Company)
            .Include(x => x.Wallet)
            .AsNoTracking();

        if (!_userContextService.IsAdminRole())
        {
            var managerCompanyIds = await _dtpDbContext.Users
                .Where(u => _userManager.IsInRoleAsync(u, ApplicationRole.MANAGER).Result)
                .Select(u => u.CompanyId)
                .Distinct()
                .ToListAsync();

            query = query.Where(x => managerCompanyIds.Contains(x.CompanyId));
        }
        
        return await query.ToListAsync();
    }

    public async Task<User?> GetUserDetailAsync(string userId) =>
        await _dtpDbContext.Users
            .AsSplitQuery()
            .Where(x => x.Id == userId)
            .Include(x => x.Company)
            .Include(x => x.Wallet)
            .FirstOrDefaultAsync();

    public async Task<string> GetUserRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
    }

    public async Task<User> GetAdmin()
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(ApplicationRole.ADMIN);

        var user = usersInRole.FirstOrDefault();

        if (user != null)
        {
            await _userManager.Users.Include(x => x.Wallet).FirstOrDefaultAsync(u => u.Id == user.Id);
        }

        return user!;
    }

    public async Task<bool> CreateUserAsync(User user, string role, Guid companyId)
    {
        if (companyId != Guid.Empty)
        {
            user.AssignCompany(companyId);
        }

        var result = await _userManager.CreateAsync(user, $"{user.UserName}{ApplicationConst.DefaultPassword}");
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }

        await AssignRole(user, role);
        return true;
    }

    public async Task<bool> UpdateProfileAsync(User user, string role)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }

        if (!string.IsNullOrEmpty(role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            await AssignRole(user, role);
        }

        return true;
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
        var changes = await _dtpDbContext.SaveChangesAsync();
        return changes > 0;
    }
}