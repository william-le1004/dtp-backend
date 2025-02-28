namespace Domain.Entities;

public partial class TourSchedule : AuditEntity
{
    public Guid TourId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MaxParticipants { get; set; }

    public double PriceChangeRate { get; set; } = 1.0;

    public string? Status { get; set; }

    public string? Remark { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
}