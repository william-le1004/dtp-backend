using Domain.Common;

namespace Domain.Entities;

public partial class Company : AuditEntity
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string TaxCode { get; set; } = null!;

    public string License { get; set; } = null!;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public virtual ICollection<User> Staffs { get; set; } = new List<User>();
}
