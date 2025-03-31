using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Queries;

public record BasketTourItemResponse
{
    public Guid TourScheduleId { get; set; }
    public Guid TourId { get; set; }
    public string TourName { get; set; }
    public bool IsTourScheduleAvailable { get; set; }
    public List<TicketResponse> Tickets { get; set; } = new();
}

public record TicketResponse
{
    public int AvailableTicket { get; set; }
    public int Capacity { get; set; }
    public bool HasAvailableTicket { get; set; }
    public int Quantity { get; set; }
    public decimal NetCost { get; set; }
    public bool IsAvailable { get; set; }
    public TicketKind TicketKind { get; set; }
}

public record GetBaskets : IRequest<IEnumerable<BasketTourItemResponse>>;

public class BasketHandler(
    IDtpDbContext context,
    ITourScheduleRepository repository,
    IUserContextService userService) 
    : IRequestHandler<GetBaskets, IEnumerable<BasketTourItemResponse>>
{
    public async Task<IEnumerable<BasketTourItemResponse>> Handle(GetBaskets request,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();

        var basket = await context.Baskets.Include(x => x.Items)
            .Include(x => x.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        if (basket is null)
        {
            return null!;
        }
        
        var tourScheduleIds = basket.Items.Select(x => x.TourScheduleId).Distinct().ToList();
        
        var tourSchedules = await repository.GetTourSchedulesByRange(tourScheduleIds);
        
        return basket.Items.GroupBy(x => x.TourScheduleId)
            .Select(x =>
            {
                var tourSchedule = tourSchedules.Single(t => t.Id == x.Key);
                return new BasketTourItemResponse()
                {
                    TourId = tourSchedule.TourId,
                    TourName = tourSchedule.Tour.Title,
                    TourScheduleId = x.Key,
                    IsTourScheduleAvailable = tourSchedule.IsAvailable(),
                    Tickets = x.Select(y =>
                    {
                        var tourScheduleTicket =
                            tourSchedule.TourScheduleTickets.Single(t => t.TicketTypeId == y.TicketTypeId);
                        return new TicketResponse()
                        {
                            Quantity = y.Quantity,
                            HasAvailableTicket = tourScheduleTicket.HasAvailableTicket(y.Quantity),
                            NetCost = tourScheduleTicket.NetCost,
                            TicketKind = tourScheduleTicket
                                .TicketType.TicketKind,
                            IsAvailable = tourScheduleTicket.IsAvailable(),
                            AvailableTicket = tourScheduleTicket.AvailableTicket,
                            Capacity = tourScheduleTicket.Capacity,
                        };
                    }).ToList(),
                };
            });
    }
}