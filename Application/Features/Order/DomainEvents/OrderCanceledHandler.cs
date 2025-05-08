using Application.Contracts.EventBus;
using Application.Contracts.Firebase;
using Application.Messaging.Order;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Order.DomainEvents;

public class OrderCanceledHandler(
    IEventBus eventBus,
    ILogger<OrderPaidHandler> logger, IFcmService fcmService
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

        await fcmService.SendNotificationAsync("Tour đã được hủy", $"Đơn hàng {notification.OrderCode} đã bị hủy.");
        logger.LogInformation($"Publish Order Cancel {notification.OrderId}");
    }
}