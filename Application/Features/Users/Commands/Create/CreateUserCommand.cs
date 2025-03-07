using Application.Common;
using MediatR;

namespace Application.Features.Users.Commands.Create;

public record CreateUserCommand(
    string Name, 
    string UserName, 
    string Email,
    string Address,
    string RoleName,
    string PhoneNumber)
    : IRequest<ApiResponse<bool>>;