using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Users.Mapping;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserDetailQuery(string Id) : IRequest<ApiResponse<UserDetailDto>>;

public class GetUserDetailQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserDetailQuery, ApiResponse<UserDetailDto>>
{
    public async Task<ApiResponse<UserDetailDto>> Handle(GetUserDetailQuery request,
        CancellationToken cancellationToken)
    {
        var result = await userRepository.GetUserDetailAsync(request.Id);
        var role = await userRepository.GetUserRole(request.Id);

        if (result == null)
            return ApiResponse<UserDetailDto>.Failure("User not found");

        return ApiResponse<UserDetailDto>.SuccessResult(result.MapToUserDetailDto(role)
        );
    }
}