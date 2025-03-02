using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Commands;

public record RemoveAllFromBasket() : IRequest;

public class RemoveAllFromBasketHandler(IDtpDbContext context) : IRequestHandler<RemoveAllFromBasket>
{
    public async Task Handle(RemoveAllFromBasket request, CancellationToken cancellationToken)
    {
        var userId = Guid.Empty;
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        basket.EmptyBasket();
        context.Baskets.Update(basket);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}