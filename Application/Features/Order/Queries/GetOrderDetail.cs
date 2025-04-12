using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Enum;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries;

public record OrderDetailResponse
{
    public string Code { get; set; }
    
    public long RefCode { get; set; }
    public string Name { get; set; }

    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string TourName { get; set; }
    public string TourThumnail { get; set; }
    public Guid TourScheduleId { get; set; }
    public DateTime TourDate { get; set; }

    public DateTime OrderDate { get; set; }

    public required List<OrderTicketResponse> OrderTickets { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrossCost { get; set; }
    public decimal NetCost { get; set; }
    
    public BookingStatus Status { get; set; }

    public string? PaymentLinkId { get; init; }
};

public record OrderTicketResponse
{
    public string Code { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal GrossCost { get; set; }
    public TicketKind TicketKind { get; set; }
}

public record GetOrderDetail(Guid Id) : IRequest<Option<OrderDetailResponse>>;

public class GetOrderByIdHandler(IDtpDbContext context, IUserContextService userService) : IRequestHandler<GetOrderDetail, Option<OrderDetailResponse>>
{
    public async Task<Option<OrderDetailResponse>> Handle(GetOrderDetail request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;

        var payment = await context.Payments.FirstOrDefaultAsync(x => x.BookingId == request.Id, cancellationToken: cancellationToken);
        var order = await context.TourBookings
            .Include(x => x.TourSchedule)
            .ThenInclude(x => x.Tour)
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.UserId == userId)
            .Select(x => new OrderDetailResponse
            {
                PaymentLinkId = payment != null ? payment.PaymentLinkId : string.Empty,
                Code = x.Code,
                Name = x.Name,
                RefCode = x.RefCode,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                TourName = x.TourSchedule.Tour.Title,
                TourScheduleId = x.TourScheduleId,
                TourThumnail = context.ImageUrls.Any(image => image.RefId == x.TourSchedule.Tour.Id)
                    ? context.ImageUrls.FirstOrDefault(image => image.RefId == x.TourSchedule.Tour.Id).Url
                    : null,
                OrderDate = x.CreatedAt,
                TourDate = x.TourSchedule.OpenDate ?? DateTime.MinValue,
                DiscountAmount = x.DiscountAmount,
                GrossCost = x.GrossCost,
                NetCost = x.NetCost(),
                Status = x.Status,
                OrderTickets = x.Tickets.Select(t => new OrderTicketResponse()
                {
                    Code = t.Code,
                    TicketTypeId = t.TicketTypeId,
                    Quantity = t.Quantity,
                    TicketKind = t.TicketType.TicketKind,
                    GrossCost = t.GrossCost
                }).ToList(),
            })
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        return order is null ? Option.None : Option.Some(order);
    }
}