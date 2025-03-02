namespace Domain.Entities;

public partial class TourSchedule : AuditEntity
{
    public Guid TourId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int AvailableTicket { get; set; }

    public double PriceChangeRate { get; set; } = 1.0;

    public string? Remark { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();

    public bool IsAvailable()
    {
        return AvailableTicket > 0 && !IsStarted();
    }

    public bool HasAvailableTicket(int quantity)
    {
        return AvailableTicket > quantity;
    }

    public bool IsStarted()
    {
        return StartDate > DateTime.Now;
    }
}