using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface ICompanyRepository
{
    Task<bool> GrantCompanyAsync(Company company);
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company?> GetCompanyAsync(Guid companyId);
    Task<bool> UpsertCompanyAsync(Company company);
    Task<bool> IsCompanyExist(string name);
    Task<string> GetCompanyName(Guid id);
    Task<bool> IsEmailCompanyExist(string gmail);
}