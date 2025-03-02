using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Basket.Queries;

public record BasketTourItemResponse
{
    public Guid TourScheduleId { get; set; }
    public bool IsAvailable { get; set; }
    public List<TicketResponse> Tickets { get; set; } = new();
}

public record TicketResponse
{
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal NetCost { get; set; }
    public double Tax { get; set; }
    public TicketKind TicketKind { get; set; }
}

public record GetBaskets : IRequest<IEnumerable<BasketTourItemResponse>>;

public class BasketHandler(IDtpDbContext context) : IRequestHandler<GetBaskets, IEnumerable<BasketTourItemResponse>>
{
    public async Task<IEnumerable<BasketTourItemResponse>> Handle(GetBaskets request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.Empty;
        // Update later when we have done the identity

        var basket = await context.Baskets.Include(x => x.Items)
            .ThenInclude(x => x.TicketType)
            .Include(x => x.Items)
            .ThenInclude(x => x.TourSchedule)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        return basket.Items.GroupBy(x => x.TourScheduleId)
            .Select(x => new BasketTourItemResponse()
            {
                TourScheduleId = x.Key,
                IsAvailable = x.First().TourSchedule.IsAvailable(),
                Tickets = x.Select(y => new TicketResponse()
                {
                    TicketTypeId = y.TicketTypeId,
                    Quantity = y.Quantity,
                    NetCost = y.TicketType.NetCost,
                    Tax = y.TicketType.Tax,
                    TicketKind = y.TicketType.TicketKind
                }).ToList(),
            });
    }
}