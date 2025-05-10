using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record ExternalTransactionResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string CompanyName { get; set; }
    public Guid CompanyId { get; set; }

    public string ExternalTransactionCode { get; set; }

    public string TransactionCode { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public ExternalTransactionType Type { get; set; }

    public ExternalTransactionStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string BankAccountNumber { get; set; }
    public string BankName { get; set; }
    public string BankAccount { get; set; }
}

public record GetExternalTransaction() : IRequest<IQueryable<ExternalTransactionResponse>>;

public class GetExternalTransactionHandler(IDtpDbContext context)
    : IRequestHandler<GetExternalTransaction, IQueryable<ExternalTransactionResponse>>
{
    public Task<IQueryable<ExternalTransactionResponse>> Handle(GetExternalTransaction request,
        CancellationToken cancellationToken)
    {
        var externalTransactions =
            context.ExternalTransaction
                .Include(x => x.User)
                .ThenInclude(x => x.Company)
                .AsSingleQuery()
                .Select(x => new ExternalTransactionResponse()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ExternalTransactionCode = x.ExternalTransactionCode,
                    TransactionCode = x.TransactionCode,
                    Description = x.Description,
                    Amount = x.Amount,
                    CompanyName = x.User.Company != null ? x.User.Company.Name : string.Empty,
                    CompanyId = x.User.Company != null ? x.User.Company.Id : Guid.Empty,
                    Status = x.Status,
                    Type = x.Type,
                    CreatedAt = x.CreatedAt,
                    BankAccountNumber = x.BankAccountNumber,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                });
        return Task.FromResult(externalTransactions);
    }
}