using System.ComponentModel.DataAnnotations;
using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Entities;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Commands;

public record WalletWithdraw : IRequest<Option<ExternalTransaction>>
{
    [Required]
    [Range(5000, double.MaxValue, ErrorMessage = "The amount must be at least 5,000.")]
    public decimal Amount { get; set; }
    public required string BankAccountNumber { get; set; }
    public required string BankName { get; set; }
    public required string BankAccount { get; set; }
}

public class WalletWithdrawHandler(
    IDtpDbContext context,
    IUserContextService service) : IRequestHandler<WalletWithdraw, Option<ExternalTransaction>>
{
    public async Task<Option<ExternalTransaction>> Handle(WalletWithdraw request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;

        var user = await context.Users.Include(x => x.Wallet)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

        if (user is not null)
        {
            var wallet = user.Wallet;
            var transaction = wallet.Withdraw(request.Amount);
            var externalTransaction = user.RequestWithdraw(transaction.TransactionCode,
                transaction.Amount, $"Rut tien {request.Amount}", 
                request.BankName, request.BankAccountNumber,request.BankAccount);
            context.Wallets.Update(wallet);
            context.ExternalTransaction.Add(externalTransaction);
            await context.SaveChangesAsync(cancellationToken);
            return externalTransaction;
        }

        return Option.None;
    }
}