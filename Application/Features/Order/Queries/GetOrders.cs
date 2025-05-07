using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Order.Dto;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries;

public record OrderResponses
{
    public Guid OrderId { get; init; }
    public string TourName { get; set; }
    public Guid TourId { get; init; }
    public string? TourThumbnail { get; set; }
    public DateTime TourDate { get; set; }
    public List<OrderTicketResponse> OrderTickets { get; init; } = new ();
    public decimal FinalCost { get; set; }
    
    public bool CanRating { get; set; }
    
    public BookingStatus Status { get; set; }
}

public record GetOrders : IRequest<IEnumerable<OrderResponses>>;

public class GetOrdersHandler(IDtpDbContext context, IUserContextService userService) : IRequestHandler<GetOrders, IEnumerable<OrderResponses>>
{
    public async Task<IEnumerable<OrderResponses>> Handle(GetOrders request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;
        var orders = await context.TourBookings
            .Include(x => x.TourSchedule)
            .ThenInclude(x => x.Tour)
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType).Where(b => b.UserId == userId)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(x=> x.UserId == userId)
            .OrderByDescending(x=> x.CreatedAt)
            .Select(x=> new OrderResponses()
            {
                OrderId = x.Id,
                TourName = x.TourSchedule.Tour.Title,
                TourId = x.TourSchedule.TourId,
                TourThumbnail = context.ImageUrls.Any(image => image.RefId == x.TourSchedule.Tour.Id)
                    ? context.ImageUrls.FirstOrDefault(image => image.RefId == x.TourSchedule.Tour.Id)!.Url
                    : null,
                TourDate = x.TourSchedule.OpenDate ?? DateTime.MinValue,
                CanRating = x.CanRatting() && !context.Ratings.Any(rating =>rating.TourBookingId == x.Id),
                OrderTickets = x.Tickets.Select(t => new OrderTicketResponse()
                {
                    Code = t.Code,
                    TicketTypeId = t.TicketTypeId,
                    Quantity = t.Quantity,
                    TicketKind = t.TicketType.TicketKind,
                    GrossCost = t.GrossCost
                }).ToList(),
                FinalCost = x.NetCost(),
                Status = x.Status,
            }).ToListAsync(cancellationToken: cancellationToken);
        return orders;
    }
}