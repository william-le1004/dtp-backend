using Domain.Enum;
using Domain.Extensions;

namespace Domain.Entities;

public class Transaction : AuditEntity
{
    public Guid WalletId { get; init; }
    
    public string TransactionCode { get; private set; }
    
    public string? Description { get; private set; }
    public Guid? ReceiveWalletId { get; private set; }

    public decimal AfterTransactionBalance { get; private set; }

    public decimal Amount { get; private set; }

    public TransactionType Type { get; private set; }

    public TransactionStatus Status { get; private set; }

    public virtual Wallet Wallet { get; private set; } = null!;
    
    public Transaction(decimal currentBalance,
        decimal amount, TransactionType type, Guid walletId, string? description = null, Guid? receiveWalletId = null)
    {
        TransactionCode = (int.Parse(DateTimeOffset.Now.ToString("fffd"))
                           + walletId.ToString("N").Substring(0, 8)).Random();
        Description = description;
        ReceiveWalletId = receiveWalletId;
        AfterTransactionBalance = currentBalance;
        Amount = amount <= 0 ? amount :
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        Type = type;
        Status = TransactionStatus.Pending;
    }

    public void AcceptWithdrawal()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't accept this transaction. Status: {Status}");
        }

        Status = TransactionStatus.Completed;
    }
    
    public void RejectWithdrawal()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't reject this transaction. Status: {Status}");
        }

        Status = TransactionStatus.Rejected;
    }

    public void TransactionCompleted()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't accept this transaction. Status: {Status}");
        }
        
        Status = TransactionStatus.Completed;
    }
    
    public void TransactionCancelled()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new AggregateException($"Can't cancel this transaction. Status: {Status}");
        }
        
        Status = TransactionStatus.Canceled;
    }
}