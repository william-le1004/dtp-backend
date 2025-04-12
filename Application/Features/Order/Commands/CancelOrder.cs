using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Payment.Events;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands;

public record CancelOrder(Guid Id, string? Remark) : IRequest<Option<Guid>>;

public class CancelOrderHandler(
    IDtpDbContext context,
    IUserContextService userService,
    IPublisher publisher)
    : IRequestHandler<CancelOrder, Option<Guid>>
{
    public async Task<Option<Guid>> Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;
        var order = await context.TourBookings.Include(x => x.TourSchedule)
            .SingleOrDefaultAsync(x => x.UserId == userId
                                       && x.Id == request.Id, cancellationToken: cancellationToken);

        if (order is not null)
        {
            if (order.IsPaid() || order.IsPaymentProcessing())
            {
                await publisher.Publish(new OrderCanceled(userId, order.Id, order.Code), cancellationToken);
            }
            order.Cancel(request.Remark);
            context.TourBookings.Update(order);

            await context.SaveChangesAsync(cancellationToken);
            return Option.Some(order.Id);
        }

        return Option.None;
    }
}