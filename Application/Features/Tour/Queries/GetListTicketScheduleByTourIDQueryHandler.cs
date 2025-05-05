using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries
{
    // DTO cho thông tin vé lịch trình của một TicketType trong một TourSchedule
    public record TicketTypeScheduleDto(
        Guid TicketTypeId,
        TicketKind TicketKind,
        decimal NetCost,
        int AvailableTicket,
        Guid TourScheduleId
    );

    // DTO nhóm theo ngày: chứa ngày và danh sách vé lịch trình trong ngày đó
    public record TicketScheduleByDayDto(
        DateOnly Day,
        List<TicketTypeScheduleDto> TicketSchedules
    );

    // Query nhận vào TourId và trả về danh sách TicketScheduleByDayDto được bọc trong ApiResponse
    public record GetListTicketScheduleByTourIDQuery(Guid TourId)
        : IRequest<ApiResponse<List<TicketScheduleByDayDto>>>;

    public class GetListTicketScheduleByTourIDQueryHandler
        : IRequestHandler<GetListTicketScheduleByTourIDQuery, ApiResponse<List<TicketScheduleByDayDto>>>
    {
        private readonly IDtpDbContext _context;

        public GetListTicketScheduleByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        // Cập nhật AvailableTicket trên mỗi TourScheduleTicket dựa vào các vé đã đặt
        private async Task MapAvailableTourScheduleTicketsAsync(TourSchedule schedule, CancellationToken ct)
        {
            // Lấy các tickets đã order (không Cancelled) cho schedule này
            var ordered = await _context.TourBookings
                .Where(b => b.TourScheduleId == schedule.Id
                            && b.Status != BookingStatus.Cancelled)
                .SelectMany(b => b.Tickets)
                .GroupBy(t => t.TicketTypeId)
                .Select(g => new { TicketTypeId = g.Key, Qty = g.Sum(t => t.Quantity) })
                .ToListAsync(ct);

            // Cập nhật
            foreach (var tkt in schedule.TourScheduleTickets.Where(t => !t.IsDeleted))
            {
                var orderedQty = ordered
                    .FirstOrDefault(x => x.TicketTypeId == tkt.TicketTypeId)?
                    .Qty ?? 0;
                tkt.CalAvailableTicket(orderedQty);
            }
        }

        public async Task<ApiResponse<List<TicketScheduleByDayDto>>> Handle(
            GetListTicketScheduleByTourIDQuery request,
            CancellationToken cancellationToken)
        {
            var tomorrow = DateTime.Now.AddDays(1);

            // 1) Lấy các schedule
            var schedules = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId
                             && !ts.IsDeleted
                             && ts.OpenDate.HasValue
                             && ts.OpenDate.Value >= DateTime.Today )
                .Include(ts => ts.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            // 2) Với mỗi schedule, cập nhật AvailableTicket
            foreach (var sch in schedules)
                await MapAvailableTourScheduleTicketsAsync(sch, cancellationToken);

            // 3) Lấy TicketKind lookup
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.TourId == request.TourId)
                .ToDictionaryAsync(tt => tt.Id, cancellationToken);

            // 4) Map về DTO, chỉ lấy các ticket không bị xóa
            var result = schedules
                .GroupBy(s => DateOnly.FromDateTime(s.OpenDate!.Value))
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g
                        .SelectMany(s => s.TourScheduleTickets.Where(t => !t.IsDeleted))
                        .Select(tst => new TicketTypeScheduleDto(
                            TicketTypeId: tst.TicketTypeId,
                            TicketKind: ticketTypes.TryGetValue(tst.TicketTypeId, out var tt)
                                           ? tt.TicketKind
                                           : TicketKind.Adult,
                            NetCost: tst.NetCost,
                            AvailableTicket: tst.AvailableTicket,
                            TourScheduleId: tst.TourScheduleId
                        ))
                        .ToList()
                ))
                .ToList();

            return ApiResponse<List<TicketScheduleByDayDto>>
                .SuccessResult(result, "Ticket schedules retrieved successfully");
        }
    }
}
