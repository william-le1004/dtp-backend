using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface ICompanyRepository
{
    Task<Company> GrantCompanyAsync(Guid id);
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company?> GetCompanyAsync(Guid id, bool noTracking = true);
    Task<bool> UpsertCompanyAsync(Company company);
    Task<bool> ExistsByNameAsync(string name);
    Task<string> GetNameByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string gmail);
}