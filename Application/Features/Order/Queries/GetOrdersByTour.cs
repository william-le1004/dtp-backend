using Application.Contracts;
using Application.Contracts.Persistence;
using Application.Features.Order.Dto;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries;

public record GetOrdersByTour(Guid Id) : IRequest<IQueryable<OrderByTourResponse>>;

public class GetOrdersByTourHandler(
    IDtpDbContext context,
    IUserContextService service) : IRequestHandler<GetOrdersByTour, IQueryable<OrderByTourResponse>>
{
    public Task<IQueryable<OrderByTourResponse>> Handle(GetOrdersByTour request, CancellationToken cancellationToken)
    {
        var companyId = service.GetCompanyId()!;

        var tour = context.Tours
            .FirstOrDefault(x => x.CompanyId == companyId && x.Id != request.Id);

        if (tour is not null)
        {
            var tourSchedules = context.TourSchedules
                .Where(x => x.TourId == request.Id).Select(x => x.Id);

            var orders = context.TourBookings
                .Include(x => x.Tickets)
                .ThenInclude(x => x.TicketType)
                .Where(x => tourSchedules.Contains(x.TourScheduleId))
                .Select(x => new OrderByTourResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    RefCode = x.RefCode,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    TourName = tour.Title,
                    TourScheduleId = x.TourScheduleId,
                    OrderDate = x.CreatedAt,
                    TourDate = x.TourSchedule.OpenDate ?? DateTime.MinValue,
                    DiscountAmount = x.DiscountAmount,
                    GrossCost = x.Tickets.Sum(t => t.GrossCost * t.Quantity),
                    Status = x.Status,
                    OrderTickets = x.Tickets.Select(t => new OrderTicketResponse()
                    {
                        Code = t.Code,
                        TicketTypeId = t.TicketTypeId,
                        Quantity = t.Quantity,
                        TicketKind = t.TicketType.TicketKind,
                        GrossCost = t.GrossCost
                    }),
                    PaymentStatus = context.Payments
                        .Where(p => p.BookingId == x.Id)
                        .Select(p => p.Status)
                        .FirstOrDefault()
                });
            return Task.FromResult(orders);
        }

        return Task.FromResult(Enumerable.Empty<OrderByTourResponse>().AsQueryable());
    }
}

public record OrderByTourResponse
{
    public Guid Id { get; init; }
    public string Code { get; set; }
    public long RefCode { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string TourName { get; set; }
    public Guid TourScheduleId { get; set; }
    public DateTime TourDate { get; set; }
    public DateTime OrderDate { get; set; }
    public IEnumerable<OrderTicketResponse> OrderTickets { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrossCost { get; set; }
    public BookingStatus Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}