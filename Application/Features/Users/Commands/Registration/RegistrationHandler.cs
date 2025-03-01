using Application.Contracts.Authentication;
using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Registration;

public class RegistrationHandler(IAuthenticationService authenticationService)
    : IRequestHandler<RegistrationCommand, RegistrationResponse>
{
    public async Task<RegistrationResponse> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var registrationRequest = new RegistrationRequestDto
            (request.Name, request.Address, request.Email, request.UserName, request.Password);
        var result = await authenticationService.RegisterAsync(registrationRequest);
        return new RegistrationResponse(result.Success, result.Message);
    }
}