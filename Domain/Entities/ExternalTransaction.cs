using Domain.Enum;
using Domain.Extensions;

namespace Domain.Entities;

public partial class ExternalTransaction : AuditEntity
{
    public string UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public string ExternalTransactionCode { get; private set; }
    
    public string BankAccountNumber { get; private set; }
    public string BankAccount { get; private set; }
    public string BankName { get; private set; }
    
    public string TransactionCode { get; private set; }

    public string? Description { get; private set; }

    public decimal Amount { get; private set; }

    public ExternalTransactionType Type { get; private set; }

    public ExternalTransactionStatus Status { get; private set; }

    public ExternalTransaction()
    {
    }
    
    public ExternalTransaction(
        string transactionCode,
        decimal amount, ExternalTransactionType type, string userId,
        string bankAccount, string bankAccountNumber, string bankName, string? description = null
    )
    {
        ExternalTransactionCode = (int.Parse(DateTimeOffset.Now.ToString("ssfff"))
                                   + userId.Substring(0, 8)).Random();
        Description = description;
        Amount = amount >= 0
            ? amount
            : throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        Type = type;
        TransactionCode = transactionCode;
        Status = ExternalTransactionStatus.Pending;
        BankAccount = bankAccount;
        BankAccountNumber = bankAccountNumber;
        BankName = bankName;
    }
    
    
    public void AcceptWithdrawal()
    {
        if (Status != ExternalTransactionStatus.Pending)
        {
            throw new AggregateException($"Can't accept this transaction. Status: {Status}");
        }

        Status = ExternalTransactionStatus.Done;
    }
    
    public void RejectWithdrawal()
    {
        if (Status != ExternalTransactionStatus.Pending)
        {
            throw new AggregateException($"Can't reject this transaction. Status: {Status}");
        }

        Status = ExternalTransactionStatus.Rejected;
    }
}