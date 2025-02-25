
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public partial class User : IdentityUser
{
    public decimal? Balance { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public sbyte? Role { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
