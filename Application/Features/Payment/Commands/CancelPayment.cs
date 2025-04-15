using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payment.Commands;

public record CancelPayment(string Id) : IRequest;

public class CancelPaymentHandler(IDtpDbContext context, IUserContextService service) : IRequestHandler<CancelPayment>
{
    public async Task Handle(CancelPayment request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;
        
        var payment = await context.Payments.Include(x=> x.Booking)
            .ThenInclude(x=> x.TourSchedule)
            .FirstOrDefaultAsync(x=> x.PaymentLinkId == request.Id 
                                     && x.Booking.UserId == userId,
                cancellationToken: cancellationToken);

        if (payment is not null)
        {
            payment.Cancel();
            payment.Booking.Cancel();
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}