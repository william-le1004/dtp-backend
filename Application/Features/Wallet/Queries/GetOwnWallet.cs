using Application.Contracts;
using Application.Contracts.Persistence;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record WalletDetailsResponse
{
    public string UserId { get; set; }
    public decimal Balance { get; set; }
}

public record GetOwnWallet() : IRequest<Option<WalletDetailsResponse>>;

public class GetOwnWalletHandler(IDtpDbContext context, IUserContextService service)
    : IRequestHandler<GetOwnWallet, Option<WalletDetailsResponse>>
{
    public async Task<Option<WalletDetailsResponse>> Handle(GetOwnWallet request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;
        var wallet = await context.Wallets.Select(
                x => new WalletDetailsResponse
                {
                    UserId = x.UserId,
                    Balance = x.Balance,
                }
            )
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        return wallet is not null ? wallet : Option.None;
    }
}