namespace Domain.Commons;

public class AuditEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; } = "System";

    public DateTime? LastModified { get; set; } = DateTime.Now;

    public string? LastModifiedBy { get; set; } = "System";

    public byte IsDeleted { get; set; } = 0;
}