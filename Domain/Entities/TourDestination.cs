namespace Domain.Entities;

public partial class TourDestination
{
    public Guid TourId { get; private set; }

    public Guid DestinationId { get; private set; }

    public DateTime StartTime { get; private set; }

    public DateTime EndTime { get; private set; }

    public int? SortOrder { get; private set; }

    public virtual Destination Destination { get; private set; } = null!;

    public virtual Tour Tour { get; private set; } = null!;

    public TourDestination(Guid tourId, Guid destinationId, DateTime startTime, DateTime endTime, int? sortOrder = null)
    {
        TourId = tourId;
        DestinationId = destinationId;
        StartTime = startTime;
        EndTime = endTime;
        SortOrder = sortOrder;
    }
}