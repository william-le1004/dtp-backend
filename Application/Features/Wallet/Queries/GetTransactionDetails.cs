using Application.Contracts;
using Application.Contracts.Persistence;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record TransactionDetailResponse
{
    public Guid TransactionId { get; init; }
}

public record GetTransactionDetails(Guid Id) : IRequest<Option<TransactionDetailResponse>>;

public class GetTransactionDetailsHandler(IDtpDbContext context, IUserContextService service)
    : IRequestHandler<GetTransactionDetails, Option<TransactionDetailResponse>>
{
    public Task<Option<TransactionDetailResponse>> Handle(GetTransactionDetails request,
        CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;
        var transaction = context.Wallets
            .Include(x => x.Transactions)
            .AsSplitQuery()
            .AsSplitQuery()
            .First(w => w.UserId == userId)
            .Transactions.Select(t => new TransactionDetailResponse
            {
                TransactionId = t.Id,
            })
            .SingleOrDefault(x => x.TransactionId == request.Id);

        return Task.FromResult(transaction is null
            ? Option.None
            : Option.Some(transaction));
    }
}