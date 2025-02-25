namespace Domain.Entities;

public partial class Wallet : AuditEntity
{
    public Guid UserId { get; set; }

    public decimal Balance { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
