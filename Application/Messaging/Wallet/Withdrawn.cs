namespace Application.Messaging.Wallet;

public record Withdrawn
{
    public string UserName { get; set; }
    
    public string Email { get; set; }

    public string ExternalTransactionCode { get; set; }

    public string TransactionCode { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}