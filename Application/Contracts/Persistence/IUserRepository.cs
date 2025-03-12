using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IUserRepository
{
    Task<bool> CreateUserAsync(User user, string role = "");
    Task<bool> UpdateProfileAsync(User user, string role = "");
    Task<bool> InactiveUserAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetUserDetailAsync(string userId);
}