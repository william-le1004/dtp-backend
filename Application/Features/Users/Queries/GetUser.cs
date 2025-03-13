using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserQuery : IRequest<ApiResponse<List<UserDto>>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<List<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetAllAsync();

        var userDtos = new List<UserDto>();

        foreach (var user in result)
        {
            var userRole = await _userRepository.GetUserRole(user.Id);
            userDtos.Add(new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.Company?.Name ?? "N/A",
                userRole,
                user.IsActive
            ));
        }

        return ApiResponse<List<UserDto>>.SuccessResult(userDtos);
    }
}