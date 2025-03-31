using Domain.Enum;

namespace Domain.Entities;

public class Payment : AuditEntity
{
    public Guid BookingId { get; private set; }

    public PaymentMethod Method { get; private set; } = PaymentMethod.PayOs;

    public PaymentStatus Status { get; private set; }

    public string? RefExternalTransactionCode { get; private set; }
    
    public decimal NetCost  { get; private set; }

    public string? RefTransactionCode { get; private set; }

    public string? PaymentLinkId { get; private set; }
    public virtual TourBooking Booking { get; private set; } = null!;

    public Payment()
    {
    }

    public Payment(Guid bookingId, string? paymentLinkId, decimal netCost)
    {
        BookingId = bookingId;
        NetCost = netCost;
        Status = PaymentStatus.Pending;
        PaymentLinkId = paymentLinkId;
    }

    public void PurchaseBooking(string transactionCode, string? externalTransactionCode = null)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new AggregateException($"Can't purchase this tour booking. Status: {Status}");
        }

        RefTransactionCode = transactionCode;
        RefExternalTransactionCode = externalTransactionCode;
        Status = PaymentStatus.Completed;

        Booking.Purchase();
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
        {
            throw new AggregateException($"Can't refund this tour booking. Status: {Status}");
        }
        
        Status = PaymentStatus.Refunded;
    }
}