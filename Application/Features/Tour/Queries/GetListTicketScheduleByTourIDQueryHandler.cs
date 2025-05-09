using Application.Common;
using Application.Contracts.Persistence;
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

        public async Task<ApiResponse<List<TicketScheduleByDayDto>>> Handle(
            GetListTicketScheduleByTourIDQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today.AddDays(1);

            // 1) Tính tổng số vé đã đặt (Paid) cho mỗi cặp (TourScheduleId, TicketTypeId)
            var bookingSums = await _context.TourBookings
                .Where(b =>
    (b.Status == BookingStatus.Paid
     || b.Status == BookingStatus.AwaitingPayment)
    && b.TourSchedule.TourId == request.TourId
    && b.TourSchedule.OpenDate.HasValue
    && b.TourSchedule.OpenDate.Value.Date >= today
)

                .SelectMany(b => b.Tickets, (b, t) => new
                {
                    b.TourScheduleId,
                    t.TicketTypeId,
                    t.Quantity
                })
                .GroupBy(x => new { x.TourScheduleId, x.TicketTypeId })
                .Select(g => new
                {
                    TourScheduleId = g.Key.TourScheduleId,
                    TicketTypeId = g.Key.TicketTypeId,
                    OrderedQty = g.Sum(x => x.Quantity)
                })
                .ToListAsync(cancellationToken);

            // Chuyển thành dictionary để lookup nhanh
            var bookedDict = bookingSums
                .ToDictionary(x => (x.TourScheduleId, x.TicketTypeId), x => x.OrderedQty);

            // 2) Lấy tất cả TourScheduleTicket cần thiết (1 call)
            var tickets = await _context.TourScheduleTicket
                .Where(tst =>
                    !tst.IsDeleted &&
                    !tst.TourSchedule.IsDeleted &&
                    tst.TourSchedule.TourId == request.TourId &&
                    tst.TourSchedule.OpenDate.HasValue &&
                    tst.TourSchedule.OpenDate.Value.Date >= today)
                .Include(tst => tst.TourSchedule)  // để lấy OpenDate
                .Include(tst => tst.TicketType)    // để lấy TicketKind
                .ToListAsync(cancellationToken);

            // 3) Map về DTO
            var result = tickets
                .GroupBy(tst => DateOnly.FromDateTime(tst.TourSchedule.OpenDate!.Value.Date))
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g.Select(tst =>
                    {
                        // tính available = Capacity - số đã đặt
                        var key = (tst.TourScheduleId, tst.TicketTypeId);
                        var ordered = bookedDict.TryGetValue(key, out var qty) ? qty : 0;
                        var available = tst.Capacity - ordered;

                        return new TicketTypeScheduleDto(
                            TicketTypeId: tst.TicketTypeId,
                            TicketKind: tst.TicketType.TicketKind,
                            NetCost: tst.NetCost,
                            AvailableTicket: available,
                            TourScheduleId: tst.TourScheduleId
                        );
                    }).ToList()
                ))
                .ToList();

            return ApiResponse<List<TicketScheduleByDayDto>>
                .SuccessResult(result, "Ticket schedules retrieved successfully");
        }
    }

}
