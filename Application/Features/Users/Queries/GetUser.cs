using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserQuery : IRequest<ApiResponse<IQueryable<UserDto>>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<IQueryable<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<IQueryable<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetAllAsync();

        var userDtos = result.Select(user => new UserDto(
            user.Id,
            user.UserName,
            user.Email,
            user.Company?.Name ?? "N/A",
            _userRepository.GetUserRole(user.Id).Result,
            user.IsActive
        )).AsQueryable();

        return ApiResponse<IQueryable<UserDto>>.SuccessResult(userDtos);
    }
}