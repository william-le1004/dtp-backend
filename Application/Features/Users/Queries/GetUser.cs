using Application.Common;
using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Users.MappingObject;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserQuery : IRequest<ApiResponse<IQueryable<UserDto>>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<IQueryable<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRedisCacheService _redisCache;

    public GetUserQueryHandler(IUserRepository userRepository, IRedisCacheService redisCache)
    {
        _userRepository = userRepository;
        _redisCache = redisCache;
    }

    public async Task<ApiResponse<IQueryable<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "GetAllUsers";
        var cachedUserList = await _redisCache.GetDataAsync<List<UserDto>>(cacheKey);
        if (cachedUserList != null)
        {
            return ApiResponse<IQueryable<UserDto>>.SuccessResult(cachedUserList.AsQueryable());
        }
        
        var users = await _userRepository.GetAllAsync();
        
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var role = await _userRepository.GetUserRole(user.Id);
            userDtos.Add(user.MapToUserDto(role));
        }

        await _redisCache.SetDataAsync(cacheKey, userDtos, TimeSpan.FromMinutes(10));

        return ApiResponse<IQueryable<UserDto>>.SuccessResult(userDtos.AsQueryable());
    }
}