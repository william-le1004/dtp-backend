using Application.Features.Users.Queries;
using Domain.Entities;

namespace Application.Features.Users.MappingObject;

public static class UserMappingExtensions
{
    public static UserDto MapToUserDto(this User entity, string role) =>
        new(
            entity.Id,
            entity.UserName ?? "N/A",
            entity.Email ?? "N/A",
            entity.Company?.Name ?? "N/A",
            role,
            entity.IsActive
        );

    public static UserDetailDto MapToUserDetailDto(this User entity, string role) =>
        new(
            entity.Id,
            entity.UserName,
            entity.Wallet.Balance,
            entity.Name,
            entity.Email,
            entity.PhoneNumber,
            entity.Address,
            entity.Company?.Name,
            role,
            entity.IsActive
        );
}