using System.Text.Json.Serialization;

namespace Domain.DataModel;

public class SoftDeleteEntity
{
    [JsonIgnore]
    public bool IsDeleted { get; set; }
}