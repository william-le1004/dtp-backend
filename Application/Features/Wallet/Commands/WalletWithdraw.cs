using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Entities;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Commands;

public record WalletWithdraw : IRequest<Option<ExternalTransaction>>
{
    public decimal Amount { get; set; }
}

public class WalletWithdrawHandler(
    IDtpDbContext context,
    IUserContextService service) : IRequestHandler<WalletWithdraw, Option<ExternalTransaction>>
{
    public async Task<Option<ExternalTransaction>> Handle(WalletWithdraw request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;

        var user = await context.Users.Include(x => x.Basket)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

        if (user is not null)
        {
            var wallet = user.Wallet;
            var transaction = wallet.Withdraw(request.Amount);
            var externalTransaction = user.RequestWithdraw(transaction.TransactionCode,
                transaction.Amount, $"Withdraw {request.Amount}");
            context.Users.Update(user);
            return externalTransaction;
        }

        return Option.None;
    }
}