using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;

    public UpdateProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var user = await _userRepository.GetUserDetailAsync(request.Id);
            if (user is null) return ApiResponse<bool>.Failure("User not found", 404);

            user.UpdateProfile(request.Name, request.Address, request.PhoneNumber, request.Email, request.UserName);

            var result = await _userRepository.UpdateProfileAsync(user, request.RoleName);
            return result ? ApiResponse<bool>.SuccessResult(true) : ApiResponse<bool>.Failure("Profile update failed");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred: {ex.Message}");
        }
    }
}