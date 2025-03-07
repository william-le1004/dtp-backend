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
        var validator = new RegistrationValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<bool>.Failure("Validation failed", 400, errors);
        }

        try
        {
            var registrationRequest = new RegistrationRequestDto(
                request.Name, request.Address, request.Email, request.UserName, request.PhoneNumber, request.Password);
            await _authenticationService.RegisterAsync(registrationRequest);
            return ApiResponse<bool>.SuccessResult(true, "User registered successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure(ex.Message, 400);
        }
    }
}