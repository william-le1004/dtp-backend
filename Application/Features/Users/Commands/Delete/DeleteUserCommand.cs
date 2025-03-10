using Application.Common;
using MediatR;

namespace Application.Features.Users.Commands.Delete;

public record DeleteUserCommand(string UserId) : IRequest<ApiResponse<bool>>;