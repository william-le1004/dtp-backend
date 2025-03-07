using Application.Common;
using MediatR;

namespace Application.Features.Users.Queries.GetDetail;

public record GetUserDetailQuery(string Id) : IRequest<ApiResponse<UserDetailDto>>;