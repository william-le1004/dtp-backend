using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Users.Mapping;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserQuery : IRequest<ApiResponse<IQueryable<UserDto>>>;

public class GetUserQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserQuery, ApiResponse<IQueryable<UserDto>>>
{
    public async Task<ApiResponse<IQueryable<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync();
        
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var role = await userRepository.GetUserRole(user.Id);
            userDtos.Add(user.MapToUserDto(role));
        }

        return ApiResponse<IQueryable<UserDto>>.SuccessResult(userDtos.AsQueryable());
    }
}