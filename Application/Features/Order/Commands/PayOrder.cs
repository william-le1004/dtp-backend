using Application.Contracts.Job;
using Application.Contracts.Persistence;
using Domain.Constants;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands;

public record PayOrder(long OrderCode, int Amount, string RefCode) : IRequest;

public class PayOrderHandler(
    IDtpDbContext context,
    IUserRepository repository,
    IHangfireStorageService storageService,
    IOrderJobService jobService) : IRequestHandler<PayOrder>
{
    public async Task Handle(PayOrder request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Booking.RefCode == request.OrderCode
                                      && x.Status == PaymentStatus.Pending,
                cancellationToken: cancellationToken);

        if (payment is null)
        {
            return;
        }

        var wallet = await context.Wallets.FirstOrDefaultAsync(x => x.UserId == payment.Booking.UserId,
            cancellationToken: cancellationToken);

        if (wallet is null)
        {
            return;
        }

        var admin = await repository.GetAdmin();
        var poolFund = admin.Wallet;
        context.Wallets.Attach(poolFund);

        var description = $"Thanh toan booking: {request.OrderCode}";

        var transactionCode = wallet.ThirdPartyPay(poolFund, request.Amount, description);
        payment.PurchaseBooking(transactionCode, request.RefCode);

        context.Payments.Update(payment);
        context.Wallets.UpdateRange(wallet, poolFund);
        await context.SaveChangesAsync(cancellationToken);
        var jobId = storageService.GetScheduleJobIdByArgId(nameof(IOrderJobService.CancelOrder),
            ApplicationConst.CancelOrderQueue,
            payment.BookingId);
        jobService.PaidCheck(jobId);
    }
}