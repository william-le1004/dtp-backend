using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record TransactionResponse
{
    public Guid Id { get; init; }
    public string? Description { get; set; }
    
    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }
    
    public string TransactionCode { get; init; }
    
    public DateTime CreatedAt { get; set; }
};

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
            .First(x => x.UserId == userId).Transactions
            .Select(x => new TransactionResponse()
            {
                Id = x.Id,
                Description = x.Description,
                Amount = x.Amount,
                CreatedAt = x.CreatedAt,
                Type = x.Type,
                TransactionCode = x.TransactionCode
            });

        return Task.FromResult(transactions.AsQueryable());
    }
}