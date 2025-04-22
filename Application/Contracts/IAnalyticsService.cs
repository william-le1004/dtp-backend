namespace Application.Contracts;

public interface IAnalyticsService
{
    Task UpdatePaymentAnalyticsAsync(PaymentAnalyticsData data);
} 