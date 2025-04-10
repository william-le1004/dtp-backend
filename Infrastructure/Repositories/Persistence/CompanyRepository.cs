using Application.Contracts;
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
    private readonly IUserContextService _userContext;

    public CompanyRepository(DtpDbContext dbContext, UserManager<User> userManager, IUserContextService userContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _userContext = userContext;
    }

    public async Task<bool> GrantCompanyAsync(Guid companyId)
    {
        var company = await _dbContext.Companies
            .Include(c => c.Staffs)
            .Include(c => c.Tours)
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
            .OrderBy(c => c.Name)
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

    public async Task<bool> UpsertCompanyAsync(Company company)
    {
        if (company.Id == Guid.Empty)
        {
            return await CreateCompanyAsync(company);
        }
    
        return await UpdateCompanyAsync(company);
    }

    public Task<bool> IsCompanyExist(string name)
    {
        return _dbContext.Companies.AnyAsync(c => c.Name == name);
    }

    private async Task<bool> CreateCompanyAsync(Company company)
    {
        var userId = _userContext.GetCurrentUserId();
        var userCompanyId = _dbContext.Users.Find(userId)?.CompanyId ?? Guid.Empty;

        if (userCompanyId != Guid.Empty)
        {
            throw new InvalidOperationException("User already belongs to a company.");
        }

        var user = await _dbContext.Users.FindAsync(userId)
                   ?? throw new KeyNotFoundException("User not found.");

        if (!company.Staffs.Contains(user))
        {
            company.Staffs.Add(user);
        }

        await _dbContext.Companies.AddAsync(company);
        return await SaveChangesAsync();
    }

    private async Task<bool> UpdateCompanyAsync(Company company)
    {
        var existingCompany = await GetCompanyAsync(company.Id)
                              ?? throw new KeyNotFoundException($"Company with ID {company.Id} not found.");

        if (existingCompany.Licensed)
        {
            throw new InvalidOperationException("Company is licensed and cannot be updated.");
        }

        _dbContext.Entry(existingCompany).CurrentValues.SetValues(company);
        return await SaveChangesAsync();
    }

    private async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}