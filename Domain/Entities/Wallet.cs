using Domain.Enum;

namespace Domain.Entities;

public class Wallet(string userId, decimal balance = 0) : AuditEntity
{
    private static decimal MinimumWithdrawAmount => 100000;
    public string UserId { get; private set; } = userId;
    public decimal Balance { get; private set; } = balance;

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public User User { get; private set; }

    public Transaction Deposit(decimal amount)
    {
        var transaction = new Transaction(Balance, amount, TransactionType.Deposit, Id);
        
        _transactions.Add(transaction);
        Balance += amount;
        
        return transaction;
    }

    public Transaction Withdraw(decimal amount)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        if (amount < MinimumWithdrawAmount)
        {
            throw new AggregateException($"The amount must be at least {MinimumWithdrawAmount}. Current Amount: {amount}.");
        }

        var transaction = new Transaction(Balance, amount, TransactionType.Withdraw, Id);
        _transactions.Add(transaction);
        Balance -= amount;
        
        return transaction;
    }

    public void Transfer(Wallet receiveWallet, decimal amount, string description)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        var transaction = new Transaction(Balance, amount, TransactionType.Transfer, Id, description);
        _transactions.Add(transaction);
        transaction.Ref(receiveWallet.Receive(amount, description, transaction.TransactionCode));
        
        Balance -= amount;
    }

    private string Receive(decimal amount, string description, string refTransactionCode, TransactionType transactionType = TransactionType.Receive)
    {
        var transaction = new Transaction(Balance, amount, transactionType, Id, description);
        transaction.Ref(refTransactionCode);
        _transactions.Add(transaction);
        
        Balance += amount;
        return transaction.TransactionCode;
    }
    
    public string ThirdPartyPay(Wallet poolFund, decimal amount, string description)
    {
        var transaction = new Transaction(Balance, amount, TransactionType.ThirdPartyPayment, Id, description);
        _transactions.Add(transaction);
        transaction.Ref(poolFund.Receive(amount, description, transaction.TransactionCode));
        
        Balance -= amount;
        return transaction.TransactionCode;
    }
    
    public void PayInApp(Wallet poolFund, decimal amount, string description)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        var transaction = new Transaction(Balance, amount, TransactionType.Payment, Id, description);
        _transactions.Add(transaction);
        transaction.Ref(poolFund.Receive(amount, description, transaction.TransactionCode));
        
        Balance -= amount;
    }
    
    public void Refund(Wallet receiveWallet, decimal amount, string description)
    {
        if (Balance < amount)
        {
            throw new AggregateException($"Insufficient funds!. Balance: {Balance}.");
        }

        var transaction = new Transaction(Balance, amount, TransactionType.Transfer, Id, description);
        _transactions.Add(transaction);
        transaction.Ref(receiveWallet.Receive(amount, description, transaction.TransactionCode, TransactionType.Refund));
        
        Balance -= amount;
    }
}