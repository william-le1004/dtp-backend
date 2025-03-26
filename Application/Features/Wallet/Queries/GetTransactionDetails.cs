using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Enum;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record TransactionDetailResponse
{
    public Guid TransactionId { get; init; }
    
    public string TransactionCode { get; set; }
    
    public string? Description { get; set; }
    public string RefTransactionCode { get; set; }

    public decimal AfterTransactionBalance { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public TransactionStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
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
                TransactionCode = t.TransactionCode,
                Description = t.Description,
                Amount = t.Amount,
                RefTransactionCode = t.RefTransactionCode,
                AfterTransactionBalance = t.AfterTransactionBalance,
                Status = t.Status,
                Type = t.Type,
                CreatedAt = t.CreatedAt
            })
            .SingleOrDefault(x => x.TransactionId == request.Id);

        return Task.FromResult(transaction is null
            ? Option.None
            : Option.Some(transaction));
    }
}