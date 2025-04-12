using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest<ApiResponse<bool>>;

public class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserDetailAsync(request.UserId);

        if (user is null)
            return ApiResponse<bool>.Failure("User not found");

        await userRepository.InactiveUserAsync(user);

        return ApiResponse<bool>.SuccessResult(true);
    }
}