using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface ICompanyRepository
{
    Task<bool> GrantCompanyAsync(Guid companyId);
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company?> GetCompanyAsync(Guid companyId);
    Task<bool> UpsertCompanyAsync(Company company, string userId = "");
}