using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
    // DTO cho thông tin vé lịch trình của một TicketType trong một TourSchedule
    public record TicketTypeScheduleDto(
        Guid TicketTypeId,
        TicketKind TicketKind,
        decimal NetCost,
        int AvailableTicket,  // Số vé khả dụng = capacity - số vé đã đặt
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
            // Lấy tất cả các TourSchedule của tour có TourId được cung cấp, chỉ lấy những schedule chưa bị xóa (IsDeleted == false)
            var schedules = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId && !ts.IsDeleted)
                .Include(ts => ts.TourScheduleTickets.Where(t => !t.IsDeleted))
                .ToListAsync(cancellationToken);

            // Lấy danh sách schedule IDs
            var scheduleIds = schedules.Select(s => s.Id).Distinct().ToList();

            // Lấy các TourBooking có trạng thái Paid cho các schedule đã được lọc
            var paidBookings = await _context.TourBookings
                .Where(tb => tb.Status == BookingStatus.Paid && scheduleIds.Contains(tb.TourScheduleId))
                .Include(tb => tb.Tickets)
                .ToListAsync(cancellationToken);

            // Nhóm số lượng vé được đặt theo cặp (TourScheduleId, TicketTypeId)
            var bookingQuantities = paidBookings
                .SelectMany(tb => tb.Tickets.Select(t => new
                {
                    tb.TourScheduleId,
                    t.TicketTypeId,
                    t.Quantity
                }))
                .GroupBy(x => new { x.TourScheduleId, x.TicketTypeId })
                .ToDictionary(
                    g => new { g.Key.TourScheduleId, g.Key.TicketTypeId },
                    g => g.Sum(x => x.Quantity)
                );

            // Tải các TicketType của tour để tra cứu TicketKind
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.TourId == request.TourId)
                .ToDictionaryAsync(tt => tt.Id, cancellationToken);

            // Nhóm các vé lịch trình theo ngày (dựa trên OpenDate của schedule, chuyển sang DateOnly)
            var result = schedules
                .Where(s => s.OpenDate.HasValue)
                .GroupBy(s => DateOnly.FromDateTime(s.OpenDate.Value))
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g.SelectMany(s => s.TourScheduleTickets)
                        .Select(tst =>
                        {
                            // Tính số lượng đã đặt, nếu không có thì 0
                            var key = new { TourScheduleId = tst.TourScheduleId, TicketTypeId = tst.TicketTypeId };
                            int booked = bookingQuantities.TryGetValue(key, out int qty) ? qty : 0;
                            // Số vé khả dụng = capacity (AvailableTicket) - số vé đã đặt
                            int available = tst.AvailableTicket - booked;

                            // Lấy TicketKind từ ticketTypes, nếu không tìm thấy thì mặc định là Adult
                            TicketKind ticketKind = ticketTypes.TryGetValue(tst.TicketTypeId, out var tt)
                                ? tt.TicketKind
                                : TicketKind.Adult;

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
