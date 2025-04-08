namespace Application.Messaging.Wallet;

public record TransactionRecordedIntegrationEvent(
    string Email,
    string UserName,
    decimal Amount,
    int TransactionType,
    DateTime CreatedDate,
    string TransactionCode,
    decimal AvailableBalance,
    string Description
    );