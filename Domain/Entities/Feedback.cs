namespace Domain.Entities;

public partial class Feedback
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public Guid UserId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
