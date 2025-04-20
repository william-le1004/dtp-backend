using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface ICompanyRepository
{
    Task<bool> GrantCompanyAsync(Company company);
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company?> GetCompanyAsync(Guid companyId, bool noTracking = true);
    Task<bool> UpsertCompanyAsync(Company company);
    Task<bool> ExistsByNameAsync(string name);
    Task<string> GetNameByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string gmail);
}