namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    public Guid UserId { get; set; }

    public Guid TourScheduleId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? Remark { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TourSchedule TourSchedule { get; set; } = null!;
}