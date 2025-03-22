using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Basket.Events;

public record OrderSubmittedEvent(string UserId, Guid TourScheduleId) : INotification;

public class OrderSubmittedHandler(IDtpDbContext context) : INotificationHandler<OrderSubmittedEvent>
{
    public Task Handle(OrderSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var basket = context.Baskets.SingleOrDefault(b => b.UserId == notification.UserId);
        if (basket is not null)
        {
            var basketItem = context.TourBasketItems.Where(i
                => i.TourScheduleId == notification.TourScheduleId
                   && i.BasketId == basket.Id).ToList();
            context.TourBasketItems.RemoveRange(basketItem);
            context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        return Task.CompletedTask;
    }
}