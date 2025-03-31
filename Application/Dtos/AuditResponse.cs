namespace Application.Dtos;

public record AuditResponse
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }
}