namespace Application.Contracts;

public record PaymentAnalyticsData(
    string OrderCode,
    decimal Amount,
    string PaymentMethod,
    string TourName,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    DateTime PaymentDate,
    string Status
); 