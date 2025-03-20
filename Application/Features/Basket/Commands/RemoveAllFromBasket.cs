using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Commands;

public record RemoveAllFromBasket() : IRequest;

public class RemoveAllFromBasketHandler(IDtpDbContext context, IUserContextService userService) : IRequestHandler<RemoveAllFromBasket>
{
    public async Task Handle(RemoveAllFromBasket request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        basket.EmptyBasket();
        context.Baskets.Update(basket);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}