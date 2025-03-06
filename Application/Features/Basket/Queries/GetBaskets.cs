using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Queries;

public record BasketTourItemResponse
{
    public Guid TourScheduleId { get; set; }
    public bool IsTourScheduleAvailable { get; set; }
    public List<TicketResponse> Tickets { get; set; } = new();
}

public record TicketResponse
{
    public int AvailableTicket { get; set; }
    public bool HasAvailableTicket { get; set; }
    public int Quantity { get; set; }
    public decimal NetCost { get; set; }
    public double Tax { get; set; }
    public bool IsAvailable { get; set; }
    public TicketKind TicketKind { get; set; }
}

public record GetBaskets : IRequest<IEnumerable<BasketTourItemResponse>>;

public class BasketHandler(IDtpDbContext context) : IRequestHandler<GetBaskets, IEnumerable<BasketTourItemResponse>>
{
    public async Task<IEnumerable<BasketTourItemResponse>> Handle(GetBaskets request,
        CancellationToken cancellationToken)
    {
        var userId = "7bd74dd8-e86a-40b8-837c-34929235d424";
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .Include(x => x.Items)
            .ThenInclude(x => x.TourSchedule)
            .ThenInclude(x => x.TourScheduleTickets)
            .ThenInclude(x => x.TicketType)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        return basket.Items.GroupBy(x => x.TourScheduleId)
            .Select(x => new BasketTourItemResponse()
            {
                TourScheduleId = x.Key,
                IsTourScheduleAvailable = x.First().TourSchedule.IsAvailable(),
                Tickets = x.Select(y =>
                {
                    var tourScheduleTicket =
                        y.TourSchedule.TourScheduleTickets.Single(t => t.TicketTypeId == y.TicketTypeId);
                    return new TicketResponse()
                    {
                        Quantity = y.Quantity,
                        HasAvailableTicket = tourScheduleTicket.HasAvailableTicket(y.Quantity),
                        NetCost = tourScheduleTicket.NetCost,
                        Tax = tourScheduleTicket.Tax,
                        TicketKind = tourScheduleTicket
                            .TicketType.TicketKind,
                        IsAvailable = tourScheduleTicket.IsAvailable(),
                        AvailableTicket = tourScheduleTicket.AvailableTicket
                    };
                }).ToList(),
            });
    }
}