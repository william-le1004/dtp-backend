using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Commands.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserDetailAsync(request.UserId);

        if (user is null)
            return ApiResponse<bool>.Failure("User not found");

        user.Deactivate();
        
        await _userRepository.InactiveUser(user);

        return ApiResponse<bool>.SuccessResult(true);
    }
}