using Application.Common;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Users.Queries.GetDetail;

public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, ApiResponse<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<UserDetailDto>> Handle(GetUserDetailQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserDetailAsync(request.Id);

        if (result == null)
            return ApiResponse<UserDetailDto>.Failure("User not found");

        var userDetail = new UserDetailDto
        {
            Id = result.Id,
            UserName = result.UserName,
            Balance = result.Wallet.Balance,
            Email = result.Email,
            PhoneNumber = result.PhoneNumber,
            Address = result.Address,
            Name = result.Name,
            CompanyName = result.Company?.Name
        };
        return ApiResponse<UserDetailDto>.SuccessResult(userDetail);
    }
}