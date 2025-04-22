using Application.Contracts.EventBus;
using Application.Messaging.Order;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Order.DomainEvents;

public class OrderCanceledHandler(
    IEventBus eventBus,
    ILogger<OrderPaidHandler> logger
) : INotificationHandler<OrderCanceled>
{
    public async Task Handle(OrderCanceled notification, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(new OrderCanceledIntegrationEvent()
        {
            OrderId = notification.OrderId,
            OrderCode = notification.OrderCode,
            FinalCost = notification.FinalCost,
            TourDate = notification.TourDate,
            TourName = notification.TourName,
            Email = notification.Email,
            OrderTickets = notification.OrderTickets.Select(x => new OrderTicketIntegrationEvent()
            {
                Code = x.Code,
                GrossCost = x.GrossCost,
                Quantity = x.Quantity,
                TicketKind = x.TicketKind
            }).ToList()
        });

        logger.LogInformation($"Publish Order Cancel {notification.OrderId}");
    }
}