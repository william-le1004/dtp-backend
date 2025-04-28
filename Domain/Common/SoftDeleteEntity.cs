using System.Text.Json.Serialization;

namespace Domain.Common;

public class SoftDeleteEntity
{
    [JsonIgnore]
    public bool IsDeleted { get; set; }
}