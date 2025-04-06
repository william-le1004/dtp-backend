using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Events.Wallet;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Wallet.DomainEvents;

public class TransactionRecordedHandler(
    IEventBus eventBus,
    IDtpDbContext context,
    ILogger<TransactionRecordedHandler> logger)
    : INotificationHandler<TransactionRecorded>
{
    public async Task Handle(TransactionRecorded notification, CancellationToken cancellationToken)
    {
        var wallet = await context.Wallets.Include(x => x.User)
            .Where(w => w.Id == notification.WalletId).Select(x => new
            {
                WalletId = x.Id,
                UserName = x.User.UserName,
                Email = x.User.Email,
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (wallet is { Email: not null })
        {
            await eventBus.PublishAsync(new TransactionRecordedIntegrationEvent(
                wallet.Email,
                wallet.UserName ?? string.Empty,
                notification.Amount,
                (int)notification.TransactionType,
                notification.CreatedDate,
                notification.TransactionCode,
                notification.AvailableBalance,
                notification.Description), cancellationToken);
            logger.LogInformation($"Publish Transaction {notification.TransactionCode} recorded");
        }
    }
}