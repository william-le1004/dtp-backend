using System.Windows.Input;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Events;

public record PaymentRefunded(decimal Amount, string UserId, string OrderCode) : INotification;

public class PaymentRefundedHandler(
    IDtpDbContext context,
    IUserRepository repository) : INotificationHandler<PaymentRefunded>
{
    public async Task Handle(PaymentRefunded notification, CancellationToken cancellationToken)
    {
        var wallet = await context.Wallets.FirstOrDefaultAsync(x => x.UserId == notification.UserId,
            cancellationToken: cancellationToken);

        var admin = await repository.GetAdmin();
        var poolFund = admin.Wallet;
        context.Wallets.Attach(poolFund);
        if (wallet is not null)
        {
            poolFund.Refund(wallet, notification.Amount,
                $"Refunded {notification.Amount} for TourBooking Code: {notification.OrderCode}");
            
            context.Wallets.UpdateRange(wallet, poolFund);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}