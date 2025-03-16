namespace Domain.Entities;

public partial class TourDestination
{
    public Guid Id { get; set; }
    public Guid TourId { get; private set; }

    public Guid DestinationId { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public int? SortOrder { get; private set; }

    public int? SortOrderByDate { get; private set; }

    public virtual Destination Destination { get; private set; } = null!;
    public virtual ICollection<DestinationActivity> DestinationActivities { get; private set; } = new List<DestinationActivity>();

    public virtual Tour Tour { get; private set; } = null!;

    public TourDestination(Guid tourId, Guid destinationId, TimeSpan startTime, TimeSpan endTime, int? sortOrder = null,
        int? sortOrderByDate = null)
    {
        Id = Guid.NewGuid();
        TourId = tourId;
        DestinationId = destinationId;
        StartTime = startTime;
        EndTime = endTime;
        SortOrder = sortOrder;
        SortOrderByDate = sortOrderByDate;
    }
}