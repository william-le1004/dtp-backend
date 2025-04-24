using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging.Wallet;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Wallet.Commands;

public record AcceptExternalTransaction(Guid Id) : IRequest;

public class AcceptExternalTransactionHandler(
    IDtpDbContext context,
    IEventBus eventBus,
    ILogger<AcceptExternalTransactionHandler> logger
) : IRequestHandler<AcceptExternalTransaction>
{
    public Task Handle(AcceptExternalTransaction request, CancellationToken cancellationToken)
    {
        var externalTransaction = context.ExternalTransaction
            .Include(x=> x.User)
            .FirstOrDefault(x=> x.Id == request.Id);
        if (externalTransaction is not null)
        {
            externalTransaction.AcceptWithdrawal();
            context.ExternalTransaction.Update(externalTransaction);
            context.SaveChangesAsync(cancellationToken);
            
            eventBus.PublishAsync(new Withdrawn
            {
                Amount = externalTransaction.Amount,
                Description = externalTransaction.Description,
                ExternalTransactionCode = externalTransaction.ExternalTransactionCode,
                TransactionCode = externalTransaction.TransactionCode,
                Email = externalTransaction.User.Email,
                UserName = externalTransaction.User.UserName,
                CreatedAt = externalTransaction.CreatedAt,
            }, cancellationToken);
            logger.LogInformation($"Publish AcceptExternalTransaction {nameof(Withdrawn)} paid {request.Id}");
        }
        return Task.CompletedTask;
    }
}
