namespace Domain.Entities;

public partial class TourDestination
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }

    public Guid DestinationId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int? SortOrder { get; set; }

    public int? SortOrderByDate { get; set; }

    public virtual Destination Destination { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;

 
}