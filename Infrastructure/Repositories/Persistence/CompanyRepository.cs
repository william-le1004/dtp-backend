using Application.Contracts.Persistence;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Persistence;

public class CompanyRepository : ICompanyRepository
{
    private readonly DtpDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public CompanyRepository(DtpDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<bool> GrantCompanyAsync(Guid companyId)
    {
        var company = await _dbContext.Companies
            .Include(c => c.Staffs)
            .Include(c => c.Tours)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId);
        company?.AcceptLicense();

        var staff = company?.Staffs.FirstOrDefault();
        if (staff == null) return false;

        await _userManager.RemoveFromRoleAsync(staff, ApplicationRole.TOURIST);
        await _userManager.AddToRoleAsync(staff, ApplicationRole.OPERATOR);

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Company>> GetCompaniesAsync()
    {
        return await _dbContext.Companies
            .Include(x => x.Staffs)
            .Include(x => x.Tours)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetCompanyAsync(Guid companyId)
    {
        return await _dbContext.Companies
            .Include(c => c.Staffs)
            .Include(c => c.Tours)
            .FirstOrDefaultAsync(c => c.Id == companyId);
    }

    public async Task<bool> UpsertCompanyAsync(Company company, string userId)
    {
        if (company.Id == Guid.Empty)
        {
            var staff = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            company.Staffs.Add(staff);
            await _dbContext.Companies.AddAsync(company);
        }
        else
        {
            _dbContext.Companies.Update(company);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
}