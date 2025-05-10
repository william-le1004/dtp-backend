using Application.Contracts.EventBus;
using Application.Contracts.Firebase;
using Application.Messaging.Order;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Order.DomainEvents;

public class OrderPaidHandler(
    IEventBus eventBus,
    ILogger<OrderPaidHandler> logger, IFcmService fcmService
) : INotificationHandler<OrderPaid>
{
    public async Task Handle(OrderPaid notification, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(new OrderPaidIntegrationEvent()
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

        await fcmService.SendNotificationAsync("Tour đã được đặt thành công", $"Đơn {notification.OrderCode} đã thanh toán thành công.");
        logger.LogInformation($"Publish Order paid {notification.OrderId}");
    }
}