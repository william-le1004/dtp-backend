

namespace Domain.Entities
{
    public class DestinationActivity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid TourDestinationId { get; private set; }
        public virtual TourDestination Destination { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public int? SortOrder { get; private set; }
        public DestinationActivity(Guid tourDestinationId, string name, TimeSpan startTime, TimeSpan endTime, int? sortOrder = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            TourDestinationId = tourDestinationId;
            StartTime = startTime;
            EndTime = endTime;
            SortOrder = sortOrder;
        }

    }
}
