namespace Domain.Entities;

public partial class Company : AuditEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string TaxCode { get; set; } = null!;

    public string License { get; set; } = null!;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();

    public virtual User? User { get; set; }
}
