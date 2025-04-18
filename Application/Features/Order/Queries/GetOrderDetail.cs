using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Order.Dto;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries;

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
                PaymentStatus = payment != null ? payment.Status : null,
                Code = x.Code,
                Name = x.Name,
                RefCode = x.RefCode,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                TourId = x.TourSchedule.TourId,
                TourName = x.TourSchedule.Tour.Title,
                TourScheduleId = x.TourScheduleId,
                TourThumbnail = context.ImageUrls.Any(image => image.RefId == x.TourSchedule.Tour.Id)
                    ? context.ImageUrls.FirstOrDefault(image => image.RefId == x.TourSchedule.Tour.Id)!.Url
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