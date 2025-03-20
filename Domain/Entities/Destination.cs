using System.Text.Json.Serialization;

namespace Domain.Entities;

public partial class Destination : AuditEntity
{
    public string Name { get; set; } = null!;

    public string Latitude { get; set; } = null!;

    public string Longitude { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<TourDestination> TourDestinations { get; set; } = new List<TourDestination>();
}