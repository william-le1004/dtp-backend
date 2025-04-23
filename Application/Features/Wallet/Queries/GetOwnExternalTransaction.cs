using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Queries;

public record GetOwnExternalTransaction() : IRequest<IQueryable<ExternalTransactionResponse>>;

public class GetOwnExternalTransactionHandler(IDtpDbContext context, IUserContextService service)
    : IRequestHandler<GetOwnExternalTransaction, IQueryable<ExternalTransactionResponse>>
{
    public Task<IQueryable<ExternalTransactionResponse>> Handle(GetOwnExternalTransaction request,
        CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;
        var externalTransactions =
            context.ExternalTransaction
                .Include(x => x.User)
                .ThenInclude(x => x.Company)
                .AsSingleQuery()
                .Where(x => x.UserId == userId)
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
                    Type = x.Type
                });
        return Task.FromResult(externalTransactions);
    }
}