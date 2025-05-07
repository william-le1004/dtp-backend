namespace Domain.Entities;

public partial class Rating : AuditEntity
{
    public Guid TourId { get; set; }

    public string UserId { get; set; }

    public int Star { get; set; }

    public string Comment { get; set; }
    public Guid ToourBookingId { get; set; }
    public virtual Tour Tour { get; set; } = null!;
    public virtual TourBooking TourBooking { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public Rating(Guid tourId, string userId, int star, string comment,Guid bookingid )
    {
        Id = Guid.NewGuid();
        TourId = tourId;
        ToourBookingId = bookingid;
        UserId = userId;
        Star = star;
        Comment = comment;
    }
    public Rating() { }
}