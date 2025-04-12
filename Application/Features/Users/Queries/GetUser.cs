using Application.Common;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Users.Mapping;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserQuery : IRequest<ApiResponse<IQueryable<UserDto>>>;

public class GetUserQueryHandler(IUserRepository userRepository, IRedisCacheService redisCache)
    : IRequestHandler<GetUserQuery, ApiResponse<IQueryable<UserDto>>>
{
    public async Task<ApiResponse<IQueryable<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "GetAllUsers";
        var cachedUserList = await redisCache.GetDataAsync<List<UserDto>>(cacheKey);
        if (cachedUserList != null)
        {
            return ApiResponse<IQueryable<UserDto>>.SuccessResult(cachedUserList.AsQueryable());
        }
        
        var users = await userRepository.GetAllAsync();
        
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var role = await userRepository.GetUserRole(user.Id);
            userDtos.Add(user.MapToUserDto(role));
        }

        await redisCache.SetDataAsync(cacheKey, userDtos, TimeSpan.FromMinutes(10));

        return ApiResponse<IQueryable<UserDto>>.SuccessResult(userDtos.AsQueryable());
    }
}