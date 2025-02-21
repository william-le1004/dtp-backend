namespace Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public decimal? Balance { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public sbyte? Role { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
