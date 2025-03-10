namespace Domain.Entities;

public partial class Feedback : AuditEntity
{
    public Guid TourId { get; set; }

    public string UserId { get; set; }

    public string? Description { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}