using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Commands;

public record RemoveTourFromBasket(Guid TourScheduleId) : IRequest;

public class RemoveTourFromBasketHandler(IDtpDbContext context) : IRequestHandler<RemoveTourFromBasket>
{
    public async Task Handle(RemoveTourFromBasket request, CancellationToken cancellationToken)
    {
        var userId = "7bd74dd8-e86a-40b8-837c-34929235d424";
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        basket.DeleteItem(request.TourScheduleId);
        context.Baskets.Update(basket);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}