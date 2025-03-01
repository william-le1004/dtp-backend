using Domain.Common;

namespace Domain.Entities;

public partial class Feedback : AuditEntity
{
    public Guid TourId { get; set; }

    public string UserId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public sbyte? IsDeleted { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
