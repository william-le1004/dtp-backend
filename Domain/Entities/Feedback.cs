namespace Domain.Entities;

public partial class Feedback : AuditEntity
{
    public Guid TourScheduleId { get; set; }

    public string UserId { get; set; }

    public string? Description { get; set; }

    public virtual TourSchedule TourSchedule { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}