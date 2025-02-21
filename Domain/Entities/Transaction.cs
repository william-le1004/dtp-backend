namespace Domain.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public Guid? BankAccount { get; set; }

    public Guid? BankAccountId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? Date { get; set; }

    public Guid? Type { get; set; }

    public string? Status { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
