using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enum;

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
    public record GetListTicketScheduleByTourIDQuery(Guid TourId) : IRequest<ApiResponse<List<TicketScheduleByDayDto>>>;

    public class GetListTicketScheduleByTourIDQueryHandler : IRequestHandler<GetListTicketScheduleByTourIDQuery, ApiResponse<List<TicketScheduleByDayDto>>>
    {
        private readonly IDtpDbContext _context;

        public GetListTicketScheduleByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<TicketScheduleByDayDto>>> Handle(GetListTicketScheduleByTourIDQuery request, CancellationToken cancellationToken)
        {
            // Lấy tất cả các TourSchedule của Tour có TourId được cung cấp, bao gồm các vé lịch trình
            var schedules = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId)
                .Include(ts => ts.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            // Lấy danh sách schedule IDs
            var scheduleIds = schedules.Select(s => s.Id).Distinct().ToList();

            // Lấy các TourBooking có trạng thái Paid cho các lịch trình đó
            var paidBookings = await _context.TourBookings
                .Where(tb => tb.Status == Domain.Enum.BookingStatus.Paid && scheduleIds.Contains(tb.TourScheduleId))
                .Include(tb => tb.Tickets)
                .ToListAsync(cancellationToken);

            // Nhóm các ticket từ các booking theo TourScheduleId và TicketTypeId, tổng hợp số lượng (Quantity)
            var bookingQuantities = paidBookings
                .SelectMany(tb => tb.Tickets.Select(t => new {
                    tb.TourScheduleId,
                    t.TicketTypeId,
                    t.Quantity
                }))
                .GroupBy(x => new { x.TourScheduleId, x.TicketTypeId })
                .ToDictionary(g => new { g.Key.TourScheduleId, g.Key.TicketTypeId }, g => g.Sum(x => x.Quantity));

            // Lấy thông tin TicketTypes của Tour để tra cứu TicketKind
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.TourId == request.TourId)
                .ToDictionaryAsync(tt => tt.Id, cancellationToken);

            // Nhóm các vé lịch trình theo ngày (dựa trên OpenDate của TourSchedule)
            var result = schedules
                .GroupBy(s => DateOnly.FromDateTime(s.OpenDate))
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g.SelectMany(s => s.TourScheduleTickets)
                        .Select(tst =>
                        {
                            // Tính số lượng đã đặt từ bookingQuantities, nếu không có thì 0
                            var key = new { TourScheduleId = tst.TourScheduleId, TicketTypeId = tst.TicketTypeId };
                            int booked = bookingQuantities.TryGetValue(key, out int qty) ? qty : 0;
                            // Số vé khả dụng = AvailableTicket trong vé lịch trình - số vé đã đặt
                            int available = tst.AvailableTicket - booked;

                            // Lấy TicketKind từ ticketTypes dictionary
                            TicketKind ticketKind = ticketTypes.TryGetValue(tst.TicketTypeId, out var tt)
                                ? tt.TicketKind
                                : TicketKind.Adult; // Mặc định nếu không tìm thấy

                            return new TicketTypeScheduleDto(
                                TicketTypeId: tst.TicketTypeId,
                                TicketKind: ticketKind,
                                NetCost: tst.NetCost,
                                AvailableTicket: available,
                                TourScheduleId: tst.TourScheduleId
                            );
                        }).ToList()
                )).ToList();

            return ApiResponse<List<TicketScheduleByDayDto>>.SuccessResult(result, "Ticket schedules retrieved successfully");
        }
    }
}
