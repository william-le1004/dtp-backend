using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        var newUser = new User(request.UserName, request.Email, request.Name, request.Address, request.PhoneNumber);

        var result = await _userRepository.CreateUser(newUser, request.RoleName);
        return result
            ? ApiResponse<bool>.SuccessResult(true, "User created successfully")
            : ApiResponse<bool>.Failure("User creation failed");
    }
}