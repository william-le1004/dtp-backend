namespace Application.Messaging.Wallet;

public record WithdrawRejected
{
    public string UserName { get; set; }
    
    public string Email { get; set; }

    public string ExternalTransactionCode { get; set; }

    public string TransactionCode { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string Remark { get; set; }
}