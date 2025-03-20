using Domain.DataModel;

namespace Domain.Common;

public class AuditEntity : SoftDeleteEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

}