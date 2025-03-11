using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        decimal NetCost,
        int AvailableTicket,
        Guid TourScheduleId
    );

    // DTO nhóm theo ngày: chứa ngày và danh sách vé lịch trình trong ngày đó
    public record TicketScheduleByDayDto(
        DateTime Day,
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
            // Lấy tất cả các TourSchedule của Tour có TourId được cung cấp, include các vé lịch trình
            var schedules = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId)
                .Include(ts => ts.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            // Nhóm các vé lịch trình theo ngày (dựa trên OpenDate của TourSchedule)
            var result = schedules.GroupBy(s => s.OpenDate.Date)
                .Select(g => new TicketScheduleByDayDto(
                    Day: g.Key,
                    TicketSchedules: g.SelectMany(s => s.TourScheduleTickets)
                        .Select(tst => new TicketTypeScheduleDto(
                            TicketTypeId: tst.TicketTypeId,
                            NetCost: tst.NetCost,
                            AvailableTicket: tst.AvailableTicket,
                            TourScheduleId: tst.TourScheduleId
                        )).ToList()
                )).ToList();

            return ApiResponse<List<TicketScheduleByDayDto>>.SuccessResult(result, "Ticket schedules retrieved successfully");
        }
    }
}
