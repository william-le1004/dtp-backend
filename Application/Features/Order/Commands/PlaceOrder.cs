using System.ComponentModel.DataAnnotations;
using Application.Contracts.Persistence;
using Application.Features.Order.Event;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands;

public record PlaceOrderCommand : IRequest<TourBooking>
{
    public Guid TourScheduleId { get; set; }
    public List<TicketRequest> Tickets { get; set; } = new();
}

public record TicketRequest
{
    public Guid TourScheduleTicketId { get; set; }
    public int Quantity { get; set; }
}

public class PlaceOrderCommandHandler(IDtpDbContext context, IPublisher publisher)
    : IRequestHandler<PlaceOrderCommand, TourBooking>
{
    public async Task<TourBooking> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = string.Empty;
        // Update later when we have done the identity
        var removeBasketEvent = new OrderSubmittedEvent(userId, request.TourScheduleId);
        var touSchedule = context.TourSchedules.Include(x => x.TourScheduleTickets)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefault(t => t.Id == request.TourScheduleId);

        var booking = new TourBooking(userId, request.TourScheduleId, touSchedule);

        foreach (var ticket in request.Tickets)
        {
            booking.AddTicket(ticket.Quantity, ticket.TourScheduleTicketId);
        }

        context.TourBookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        await publisher.Publish(removeBasketEvent, cancellationToken);

        return booking;
    }
}