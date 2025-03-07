using Application.Common;
using MediatR;

namespace Application.Features.Users.Queries.Get;

public record GetUserQuery : IRequest<ApiResponse<List<UserDto>>>;