using Application.Common;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string Id,
    string UserName,
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    string RoleName
) : IRequest<ApiResponse<bool>>;