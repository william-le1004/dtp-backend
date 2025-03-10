using System.ComponentModel.DataAnnotations;
using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Order.Event;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands;

public record PlaceOrderCommand : IRequest<TourBooking>
{
    [Required] public Guid TourScheduleId { get; set; }
    [Required] public required string Name { get; set; }

    [Required] [Phone] public required string PhoneNumber { get; set; }
    [Required] [EmailAddress] public required string Email { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    [Required] public List<TicketRequest> Tickets { get; set; } = new();
}

public abstract record TicketRequest
{
    [Required] public Guid TicketTypeId { get; set; }
    [Required] public int Quantity { get; set; }
}

public class PlaceOrderCommandHandler(IDtpDbContext context, IPublisher publisher, IUserContextService userService)
    : IRequestHandler<PlaceOrderCommand, TourBooking>
{
    public async Task<TourBooking> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;
        
        var removeBasketEvent = new OrderSubmittedEvent(userId, request.TourScheduleId);
        var touSchedule = context.TourSchedules.Include(x => x.TourScheduleTickets)
            .AsSplitQuery()
            .AsNoTracking()
            .Single(t => t.Id == request.TourScheduleId);

        var booking = new TourBooking(userId, request.TourScheduleId, touSchedule,
            request.Name, request.PhoneNumber, request.Email);

        foreach (var ticket in request.Tickets)
        {
            booking.AddTicket(ticket.Quantity, ticket.TicketTypeId);
        }

        context.TourBookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        await publisher.Publish(removeBasketEvent, cancellationToken);

        return booking;
    }
}