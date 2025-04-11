namespace Domain.Entities;

public partial class Rating : AuditEntity
{
    public Guid TourId { get; set; }

    public string UserId { get; set; }

    public int Star { get; set; }
    public List<string> Images { get; set; } = new List<string>();

    public string Comment { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}