using MediatR;

namespace Application.Messaging.Tour;

public record TourCancelled(
    string CompanyName,
    string TourTitle,
    string BookingCode,
    string CustomerName,
    DateTime StartDate,
    string Remark,
    decimal PaidAmount,
    decimal RefundAmount
) : INotification; 