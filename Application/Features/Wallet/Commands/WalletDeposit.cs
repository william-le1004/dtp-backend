using MediatR;

namespace Application.Features.Wallet.Commands;

public record WalletDeposit() : IRequest;

public class WalletDepositHandler : IRequestHandler<WalletDeposit>
{
    public Task Handle(WalletDeposit request, CancellationToken cancellationToken)
    {
       return Task.CompletedTask;
    }
}