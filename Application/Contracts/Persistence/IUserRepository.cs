using Application.Dtos;
using Application.Features.Users.Queries.Get;
using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IUserRepository
{
    Task<bool> CreateUser(User user, string role = "");
    Task<bool> UpdateProfile(User user, string role = "");
    Task<bool> InactiveUser(User user);
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetUserDetailAsync(string userId);
}