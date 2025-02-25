namespace Domain.Entities;

public partial class Destination : AuditEntity
{
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<TourDestination> TourDestinations { get; set; } = new List<TourDestination>();
}
