using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Extensions;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Persistence;

public class UserRepository(
    DtpDbContext dtpDbContext,
    UserManager<User> userManager,
    IUserContextService userContextService)
    : IUserRepository
{
    public async Task<bool> InactiveUserAsync(User user)
    {
        user.Deactivate();
        return await SaveChangesIfNeededAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        IQueryable<User> query = dtpDbContext.Users
            .AsSplitQuery()
            .Include(x => x.Company)
            .Include(x => x.Wallet)
            .AsNoTracking();

        if (!userContextService.IsAdminRole())
        {
            var managerCompanyIds = await dtpDbContext.Users
                .Where(u => userManager.IsInRoleAsync(u, ApplicationRole.MANAGER).Result)
                .Select(u => u.CompanyId)
                .Distinct()
                .ToListAsync();

            query = query.Where(x => managerCompanyIds.Contains(x.CompanyId));
        }
        
        return await query.ToListAsync();
    }

    public async Task<User?> GetUserDetailAsync(string userId, bool noTracking) =>
        await dtpDbContext.Users
            .AsSplitQuery()
            .Include(x => x.Company)
            .Include(x => x.Wallet)
            .ApplyNoTracking(noTracking)
            .FirstOrDefaultAsync(x => x.Id == userId);

    public async Task<string> GetUserRole(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return (await userManager.GetRolesAsync(user)).FirstOrDefault();
    }

    public async Task<User> GetAdmin()
    {
        var usersInRole = await userManager.GetUsersInRoleAsync(ApplicationRole.ADMIN);

        var user = usersInRole.FirstOrDefault();

        if (user != null)
        {
            await userManager.Users.Include(x => x.Wallet).FirstOrDefaultAsync(u => u.Id == user.Id);
        }

        return user!;
    }

    public async Task<User> GetOperatorByCompanyId(Guid companyId)
    {
        var usersInRole = await userManager.GetUsersInRoleAsync(ApplicationRole.OPERATOR);

        var user = usersInRole.FirstOrDefault(x => x.CompanyId == companyId);

        if (user != null)
        {
            await userManager.Users.Include(x => x.Wallet)
                .Include(x=> x.Company)
                .FirstOrDefaultAsync(u => u.Id == user.Id && u.CompanyId == companyId);
        }

        return user!;
    }

    public async Task<bool> CreateUserAsync(User user, string role, Guid companyId)
    {
        if (companyId != Guid.Empty)
        {
            user.AssignCompany(companyId);
        }

        var result = await userManager.CreateAsync(user, $"{user.UserName}{ApplicationConst.DefaultPassword}");
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        await AssignRole(user, role);
        return true;
    }

    public async Task<bool> UpdateProfileAsync(User user, string role)
    {
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        if (!string.IsNullOrEmpty(role))
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Contains(role))
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }

            await AssignRole(user, role);
        }

        return true;
    }

    private async Task AssignRole(User user, string role)
    {
        var currentRole = userContextService.GetCurrentUserRoles();
        
        if (currentRole.Contains(ApplicationRole.ADMIN) || currentRole.Contains(ApplicationRole.OPERATOR))
        {
            var roleToAssign = currentRole.Contains(ApplicationRole.ADMIN) ? role : ApplicationRole.MANAGER;
            await userManager.AddToRoleAsync(user, roleToAssign);
        }
    }

    private async Task<bool> SaveChangesIfNeededAsync()
    {
        var changes = await dtpDbContext.SaveChangesAsync();
        return changes > 0;
    }
}