namespace Domain.Entities;

public partial class Payment : AuditEntity
{
    public Guid BookingId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? TransactionDate { get; set; }

    public virtual TourBooking Booking { get; set; } = null!;
}
