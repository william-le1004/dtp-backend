using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;

namespace Application.Features.Order.Commands;

public record PayOrder(string OrderCode) : IRequest;

public class PayOrderHandler(IDtpDbContext context, IUserContextService userService) : IRequestHandler<PayOrder>
{
    public Task Handle(PayOrder request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;

        var order = context.TourBookings.SingleOrDefault(b => b.UserId == userId && b.Code == request.OrderCode);

        if (order is not null)
        {
            order.PurchaseBooking();
        }

        return Task.CompletedTask;
    }
}