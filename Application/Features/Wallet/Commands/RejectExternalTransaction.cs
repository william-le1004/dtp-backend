using System.Text.Json.Serialization;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging.Wallet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Wallet.Commands;

public record RejectExternalTransaction : IRequest
{
    public string Remark { get; set; }
    
    [JsonIgnore]
    public Guid Id { get; set; }
}

public class RejectExternalTransactionHandler(
    IDtpDbContext context,
    IEventBus eventBus,
    ILogger<AcceptExternalTransactionHandler> logger
    ) : IRequestHandler<RejectExternalTransaction>
{
    public Task Handle(RejectExternalTransaction request, CancellationToken cancellationToken)
    {
        var externalTransaction = context.ExternalTransaction
            .Include(x=> x.User)
            .ThenInclude(x=> x.Wallet)
            .FirstOrDefault(x=> x.Id == request.Id);
        if (externalTransaction is not null)
        {
            externalTransaction.RejectWithdrawal();
            externalTransaction.User.Wallet.RefundRequest(externalTransaction.Amount, 
                $"Yêu cầu rút tiền {externalTransaction.ExternalTransactionCode} bị từ chối");
            context.ExternalTransaction.Update(externalTransaction);
            context.SaveChangesAsync(cancellationToken);
            
            eventBus.PublishAsync(new WithdrawRejected
            {
                Amount = externalTransaction.Amount,
                Description = externalTransaction.Description,
                ExternalTransactionCode = externalTransaction.ExternalTransactionCode,
                TransactionCode = externalTransaction.TransactionCode,
                Email = externalTransaction.User.Email,
                UserName = externalTransaction.User.UserName,
                CreatedAt = externalTransaction.CreatedAt,
                Remark = request.Remark
            }, cancellationToken);
            logger.LogInformation($"Publish RejectExternalTransaction {nameof(WithdrawRejected)} paid {request.Id}");
        }
        return Task.CompletedTask;
    }
}