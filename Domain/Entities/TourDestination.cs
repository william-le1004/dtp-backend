namespace Domain.Entities;

public partial class TourDestination
{
    public Guid TourId { get; set; }

    public Guid DestinationId { get; set; }

    public int? SortOrder { get; set; }

    public virtual Destination Destination { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
