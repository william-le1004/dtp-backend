using Domain.Enum;
using MediatR;

namespace Domain.Events;

public record TransactionRecorded(
    Guid WalletId,
    decimal Amount, 
    string Description,
    TransactionType TransactionType,
    DateTime CreatedDate,
    string TransactionCode,
    decimal AvailableBalance
    ) : INotification;