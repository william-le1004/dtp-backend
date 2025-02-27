using Domain.DataModel;

namespace Domain.Entities;

public partial class TourDestination
{
    public Guid TourId { get; set; }

    public Guid DestinationId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int? SortOrder { get; set; }

    public List<ImageUrl> ImageUrls { get; set; } = new();

    public virtual Destination Destination { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}