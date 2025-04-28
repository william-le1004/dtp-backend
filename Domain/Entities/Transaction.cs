using Domain.Enum;
using Domain.Events;
using Domain.Extensions;

namespace Domain.Entities;

public class Transaction : Entity
{
    public Guid WalletId { get; init; }

    public string TransactionCode { get; private set; }

    public string? Description { get; private set; }
    public string? RefTransactionCode { get; private set; }

    public decimal AfterTransactionBalance { get; private set; }

    public decimal Amount { get; private set; }

    public TransactionType Type { get; private set; }

    public TransactionStatus Status { get; private set; }

    public virtual Wallet Wallet { get; private set; } = null!;

    public Transaction()
    {
    }

    public Transaction(decimal currentBalance,
        decimal amount, TransactionType type, Guid walletId, string? description = null)
    {
        TransactionCode = (int.Parse(DateTimeOffset.Now.ToString("fffd"))
                           + walletId.ToString("N").Substring(0, 8)).Random();
        Description = description;
        AfterTransactionBalance = CalAfterTransactionBalance(currentBalance, amount, type);
        Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        Type = type;
        Status = TransactionStatus.Pending;
        AddTransactionRecordedDomainEvent(walletId, amount, description, type, CreatedAt, TransactionCode, AfterTransactionBalance);
    }

    public void Ref(string refTransactionCode)
    {
        if (Status is not TransactionStatus.Pending)
        {
            throw new ArgumentException("Status must be Pending.", nameof(Status));
        }

        if (Type is TransactionType.Withdraw or TransactionType.Deposit)
        {
            throw new ArgumentException("Only transfer transactions are supported.", nameof(refTransactionCode));
        }

        RefTransactionCode = refTransactionCode;
    }

    public void TransactionCompleted()
    {
        if (Status is not TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't accept this transaction. Status: {Status}");
        }

        Status = TransactionStatus.Completed;
    }

    public void TransactionCancelled()
    {
        if (Status is not TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't cancel this transaction. Status: {Status}");
        }

        Status = TransactionStatus.Canceled;
    }

    private void AddTransactionRecordedDomainEvent(Guid walletId,
        decimal amount,
        string? description,
        TransactionType transactionType,
        DateTime createdDate,
        string transactionCode, decimal availableBalance)
    {
        var orderStartedDomainEvent = new TransactionRecorded(walletId, amount, description ?? string.Empty,
            transactionType, createdDate,
            transactionCode, availableBalance);

        AddDomainEvent(orderStartedDomainEvent);
    }

    private decimal CalAfterTransactionBalance(decimal balance, decimal amount, TransactionType type)
    {
        return type switch
        {
            TransactionType.Transfer or TransactionType.Payment or TransactionType.Withdraw => balance - amount,
            TransactionType.Receive or TransactionType.Refund or TransactionType.Deposit => balance + amount,
            TransactionType.ThirdPartyPayment => balance,
            _ => balance
        };
    }
}