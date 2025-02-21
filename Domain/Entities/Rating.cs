namespace Domain.Entities;

public partial class Rating
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public Guid UserId { get; set; }

    public int? Rating1 { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
