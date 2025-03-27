using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands;

public record PayOrder(long OrderCode, int Amount, string RefCode) : IRequest;

public class PayOrderHandler(
    IDtpDbContext context,
    IUserContextService userService,
    IUserRepository repository) : IRequestHandler<PayOrder>
{
    public async Task Handle(PayOrder request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;

        var payment = await context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Booking.UserId == userId && x.Booking.RefCode == request.OrderCode,
                cancellationToken: cancellationToken);
        var wallet = await context.Wallets.FirstOrDefaultAsync(x => x.UserId == userId,
            cancellationToken: cancellationToken);

        var admin = await repository.GetAdmin();
        var poolFund = admin.Wallet;
        context.Wallets.Attach(poolFund);

        if (payment != null && wallet != null)
        {
            var description = payment.PurchaseBooking(request.RefCode);
            wallet.ThirdPartyPay(poolFund, request.Amount, description);
            
            context.Payments.Update(payment);
            context.Wallets.UpdateRange(wallet, poolFund);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}