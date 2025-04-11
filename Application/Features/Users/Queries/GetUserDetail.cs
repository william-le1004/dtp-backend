using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Users.MappingObject;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserDetailQuery(string Id) : IRequest<ApiResponse<UserDetailDto>>;

public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, ApiResponse<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<UserDetailDto>> Handle(GetUserDetailQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserDetailAsync(request.Id);
        var role = await _userRepository.GetUserRole(request.Id);

        if (result == null)
            return ApiResponse<UserDetailDto>.Failure("User not found");

        return ApiResponse<UserDetailDto>.SuccessResult(result.MapToUserDetailDto(role)
        );
    }
}