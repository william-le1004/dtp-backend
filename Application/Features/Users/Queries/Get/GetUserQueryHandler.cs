using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Queries.Get;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<List<UserDto>>>
{
    IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<List<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetAll();

        var userDtos = result.Select(user => new UserDto(
            user.Id,
            user.UserName,
            user.Email,
            user.Company?.Name ?? "No Role"
        )).ToList();
        
        return ApiResponse<List<UserDto>>.SuccessResult(userDtos);
    }
}