using MediatR;

namespace Application.Features.Wallet.Commands;

public record WalletTransfer() : IRequest;

public class WalletTransferHandler : IRequestHandler<WalletTransfer>
{
    public Task Handle(WalletTransfer request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

