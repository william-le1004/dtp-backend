using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record TransactionResponse();

public record GetTransactionHistory : IRequest<IQueryable<TransactionResponse>>;

public class GetTransactionHistoryHandler(IDtpDbContext context, IUserContextService service)
    : IRequestHandler<GetTransactionHistory, IQueryable<TransactionResponse>>
{
    public Task<IQueryable<TransactionResponse>> Handle(GetTransactionHistory request,
        CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;

        var transactions = context.Wallets
            .Include(x => x.Transactions)
            .AsNoTracking()
            .AsSplitQuery()
            .First(x => x.UserId == userId).Transactions.Select(x => new TransactionResponse());

        return Task.FromResult(transactions.AsQueryable());
    }
}