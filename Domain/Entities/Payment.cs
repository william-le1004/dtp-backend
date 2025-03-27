using Domain.Enum;

namespace Domain.Entities;

public partial class Payment : AuditEntity
{
    public Guid BookingId { get; private set; }

    public PaymentMethod Method { get; private set; } = PaymentMethod.PayOs;

    public PaymentStatus Status { get; private set; }

    public string? RefTransactionCode { get; private set; }
    
    public string? PaymentLinkId { get; private set; }
    public virtual TourBooking Booking { get; private set; } = null!;

    public Payment()
    {
    }

    public Payment(Guid bookingId, string? paymentLinkId)
    {
        BookingId = bookingId;
        Status = PaymentStatus.Pending;
        PaymentLinkId = paymentLinkId;
    }

    public string PurchaseBooking(string transactionCode)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new AggregateException($"Can't purchase this tour booking. Status: {Status}");
        }

        RefTransactionCode = transactionCode;
        Status = PaymentStatus.Completed;

        Booking.Purchase();
        
        return $"Thanh toan booking: {Booking.Code}";
    }
}