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
            // Lấy tất cả các TourSchedule của Tour, chỉ lấy các schedule chưa bị xóa (IsDeleted == false)
            var schedules = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId && !ts.IsDeleted)
                .Include(ts => ts.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            // Lấy danh sách các TourScheduleTicket mà cũng chưa bị xóa
            var scheduleTicketIds = schedules
                .SelectMany(s => s.TourScheduleTickets)
                .Where(t => !t.IsDeleted)
                .Select(t => t.TourScheduleId)
                .Distinct()
                .ToList();

            // Lấy các TourBooking có trạng thái Paid cho các TourSchedule đã được lọc
            var paidBookings = await _context.TourBookings
                .Where(tb => tb.Status == Domain.Enum.BookingStatus.Paid && scheduleTicketIds.Contains(tb.TourScheduleId))
                .Include(tb => tb.Tickets)
                .ToListAsync(cancellationToken);

            // Nhóm các vé đã được đặt theo cặp (TourScheduleId, TicketTypeId)
            var bookingQuantities = paidBookings
                .SelectMany(tb => tb.Tickets.Select(t => new
                {
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
                .Where(s => s.OpenDate.HasValue)
                .GroupBy(s => DateOnly.FromDateTime(s.OpenDate.Value))
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g.SelectMany(s => s.TourScheduleTickets.Where(t => !t.IsDeleted))
                                      .Select(tst =>
                                      {
                                          // Tính số lượng đã đặt từ bookingQuantities, nếu không có thì 0
                                          var key = new { TourScheduleId = tst.TourScheduleId, TicketTypeId = tst.TicketTypeId };
                                          int booked = bookingQuantities.TryGetValue(key, out int qty) ? qty : 0;
                                          // Số vé khả dụng = AvailableTicket trong vé lịch trình - số vé đã đặt
                                          int available = tst.Capacity - booked;

                                          // Lấy TicketKind từ ticketTypes; nếu không tìm thấy, mặc định là Adult
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
