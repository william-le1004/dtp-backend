using Domain.Common;
using Infrastructure.Contexts;
using MediatR;

namespace Infrastructure.Common.Extensions;

public static class MediatrExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DtpDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any());

        var entityEntries = domainEntities.ToList();
        
        var domainEvents = entityEntries
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        entityEntries.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}