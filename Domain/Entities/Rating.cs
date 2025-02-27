namespace Domain.Entities;

public partial class Rating : AuditEntity
{
    public Guid TourId { get; set; }

    public Guid UserId { get; set; }

    public int Star { get; set; }
    
    public string Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public sbyte? IsDeleted { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}