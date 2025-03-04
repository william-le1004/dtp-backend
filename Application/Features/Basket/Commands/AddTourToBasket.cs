using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Commands;

public record BasketItemRequest(Guid TourScheduleId, Guid TourScheduleTicketId, int Units = 1);

public record AddTourToBasket(List<BasketItemRequest> Items) : IRequest;

public class AddTourToBasketHandler(IDtpDbContext context) : IRequestHandler<AddTourToBasket>
{
    public async Task Handle(AddTourToBasket request, CancellationToken cancellationToken)
    {
        var userId = Guid.Empty;
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .Include(x => x.Items)
            .ThenInclude(x => x.TourSchedule)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);


        if (basket is not null)
        {
            foreach (var item in request.Items)
            {
                basket.AddItem(item.TourScheduleId, item.TourScheduleTicketId, item.Units);
            }

            context.Baskets.Update(basket);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}