using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Wallet.Events;
using Domain.DataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payment.Events;

public record OrderCanceled(string UserId, Guid OrderId, string OrderCode) : INotification;

public class OrderCanceledHandler(IDtpDbContext context, IPublisher publisher, ISystemSettingService setting)
    : INotificationHandler<OrderCanceled>
{
    public async Task Handle(OrderCanceled notification, CancellationToken cancellationToken)
    {
        var payment = await context.Payments.Include(x => x.Booking)
            .ThenInclude(x => x.TourSchedule)
            .FirstOrDefaultAsync(x => x.BookingId == notification.OrderId,
                cancellationToken: cancellationToken);

        var refundFee = await setting.GetSettingAsync(SettingKey.CancelFee);
        var freeCancel = await setting.GetSettingAsync(SettingKey.FreeCancellationPeriod);
        var nonRefund = await setting.GetSettingAsync(SettingKey.NonRefundablePeriod);

        if (payment is not null)
        {
            if (!payment.IsPaid() || !payment.Booking.TourSchedule.IsBeforeStartDate((int)nonRefund.SettingValue))
            {
                payment.Cancel();
            }
            else
            {
                decimal refundAmount;
                if (payment.Booking.IsFreeCancellationPeriod((int)freeCancel.SettingValue))
                {
                    refundAmount = payment.NetCost;
                    await publisher.Publish(new PaymentRefunded(refundAmount, notification.UserId,
                        notification.OrderCode), cancellationToken);
                    payment.Refund();
                }
                else
                {
                    refundAmount = payment.NetCost * ((decimal)refundFee.SettingValue / 100);
                    await publisher.Publish(new PaymentRefunded(refundAmount, notification.UserId,
                        notification.OrderCode), cancellationToken);
                    payment.Refund();
                }
            }
        }

        context.Payments.Update(payment);
        await context.SaveChangesAsync(cancellationToken);
    }
}