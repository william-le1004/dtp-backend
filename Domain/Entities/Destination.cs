namespace Domain.Entities;

public partial class Destination : AuditEntity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<TourDestination> TourDestinations { get; set; } = new List<TourDestination>();
}
