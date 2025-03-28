using Application.Contracts.Persistence;
using Application.Features.Wallet.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payment.Events;

public record OrderCanceled(string UserId, Guid OrderId, string OrderCode) : INotification;

public class OrderCanceledHandler(IDtpDbContext context, IPublisher publisher) : INotificationHandler<OrderCanceled>
{
    public async Task Handle(OrderCanceled notification, CancellationToken cancellationToken)
    {
        var payment = await context.Payments.Include(x => x.Booking)
            .ThenInclude(x => x.TourSchedule)
            .FirstOrDefaultAsync(x => x.BookingId == notification.OrderId,
                cancellationToken: cancellationToken);

        if (payment is not null)
        {
            decimal refundAmount;
            if (payment.Booking.IsFreeCancellationPeriod())
            {
                refundAmount = payment.NetCost;
                await publisher.Publish(new PaymentRefunded(refundAmount, notification.UserId,
                    notification.OrderCode), cancellationToken);
                payment.Refund();
                context.Payments.Update(payment);
                await context.SaveChangesAsync(cancellationToken);
            }
            else if (payment.Booking.TourSchedule.IsBeforeStartDate(4))
            {
                refundAmount = payment.NetCost * 0.7m;
                await publisher.Publish(new PaymentRefunded(refundAmount, notification.UserId,
                    notification.OrderCode), cancellationToken);
                payment.Refund();
                context.Payments.Update(payment);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}