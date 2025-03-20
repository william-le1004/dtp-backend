using Domain.Enum;

namespace Domain.Entities;

public class Wallet(string userId, decimal balance = 0) : AuditEntity
{
    public string UserId { get; private set; } = userId;
    public decimal Balance { get; private set; } = balance;

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public User User { get; private set; }

    public void Deposit(decimal amount)
    {
        _transactions.Add(new Transaction(Balance, amount, TransactionType.Deposit, Id));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        _transactions.Add(new Transaction(Balance, amount, TransactionType.Withdraw, Id));
        Balance -= amount;
    }

    public void Transfer(Wallet receiveWallet, decimal amount, string destination)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        _transactions.Add(new Transaction(Balance, amount, TransactionType.Transfer, Id, destination,
            receiveWallet.Id));
        Balance -= amount;
        receiveWallet.Receive(amount, destination, receiveWallet.Id);
    }

    private void Receive(decimal amount, string destination, Guid sendingWalletId)
    {
        _transactions.Add(new Transaction(Balance, amount, TransactionType.Receive, Id, destination,
            sendingWalletId));
        
        Balance += amount;
    }
}