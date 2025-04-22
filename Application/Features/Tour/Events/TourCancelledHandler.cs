using Application.Contracts;
using Application.Messaging.Tour;
using MediatR;

namespace Application.Features.Tour.Events;

public class TourCancelledHandler : INotificationHandler<TourCancelled>
{
    private readonly IAnalyticsService _analyticsService;

    public TourCancelledHandler(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task Handle(TourCancelled notification, CancellationToken cancellationToken)
    {
        // Gửi thông tin cancel tour qua analytics service
        await _analyticsService.UpdatePaymentAnalyticsAsync(new PaymentAnalyticsData(
            OrderCode: notification.BookingCode,
            Amount: notification.PaidAmount,
            PaymentMethod: "Refund",
            TourName: notification.TourTitle,
            CustomerName: notification.CustomerName,
            CustomerEmail: string.Empty, // Cần thêm vào event nếu cần
            CustomerPhone: string.Empty, // Cần thêm vào event nếu cần
            PaymentDate: DateTime.UtcNow,
            Status: "Cancelled"
        ));
    }
} 