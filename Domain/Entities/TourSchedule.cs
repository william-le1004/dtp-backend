namespace Domain.Entities;

public partial class TourSchedule : AuditEntity
{
    public Guid TourId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MaxParticipants { get; set; }

    public List<Ticket> Tickets { get; set; } = new();

    public string? Status { get; set; }

    public string? Remark { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
}