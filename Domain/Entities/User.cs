using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public partial class User : IdentityUser
{
    public decimal? Balance { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Wallet Wallet { get; set; }
}