using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public partial class User : IdentityUser<Guid>
{
    public decimal? Balance { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public sbyte? Role { get; set; }

    public Guid? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}