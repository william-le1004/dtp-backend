namespace Domain.Entities;

public partial class Wallet : AuditEntity
{
    public string UserId { get; private set; }
    public decimal Balance { get; private set; } = 0;

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public virtual User User { get; set; } = null!;

    public Wallet(string userId) => UserId = userId;

    public void AddFunds(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        Balance += amount;
    }

    public bool DeductFunds(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        if (Balance < amount) return false;

        Balance -= amount;
        return true;
    }

    public void AddTransaction(Transaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        _transactions.Add(transaction);
    }
}
