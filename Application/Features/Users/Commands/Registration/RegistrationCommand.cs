using Application.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Registration;

public record RegistrationCommand(string Name, string Address, string Email, string UserName, string Password) 
    : IRequest<RegistrationResponse>;