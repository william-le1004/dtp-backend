using Application.Common;
using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Registration;

public class RegistrationHandler
    : IRequestHandler<RegistrationCommand, ApiResponse<bool>>
{
    private IAuthenticationService _authenticationService;

    public RegistrationHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<ApiResponse<bool>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var validator = new RegistrationCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }
        
        var registrationRequest = new RegistrationRequestDto
            (request.Name, request.Address, request.Email, request.UserName, request.PhoneNumber, request.Password);
        var result = await _authenticationService.RegisterAsync(registrationRequest);
        if (!result.Success)
        {
            return ApiResponse<bool>.Failure(result.Message, 400);
        }

        return ApiResponse<bool>.SuccessResult(true, "User registered successfully", 201);
    }
}