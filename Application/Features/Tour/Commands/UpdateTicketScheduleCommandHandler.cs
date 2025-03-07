using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Tour.Commands
{
    // Command cập nhật giá và số lượng vé cho các vé lịch trình (TourScheduleTicket)
    // Nếu không gửi ngày (StartDate, EndDate) thì cập nhật cho tất cả và cập nhật luôn giá mặc định cho TicketType
    public record UpdateTicketScheduleCommand(
        Guid TourId,
        decimal? NewNetCost,
        int? NewAvailableTicket,
        DateTime? StartDate,  // Nếu có, chỉ áp dụng cho những vé của TourSchedule có StartDate nằm trong khoảng này
        DateTime? EndDate
    ) : IRequest<ApiResponse<string>>;

    public class UpdateTicketScheduleHandler : IRequestHandler<UpdateTicketScheduleCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTicketScheduleHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(UpdateTicketScheduleCommand request, CancellationToken cancellationToken)
        {
            // Cast _context sang DbContext để sử dụng Entry
            var dbContext = _context as DbContext;
            if (dbContext == null)
                throw new Exception("The IDtpDbContext instance is not a DbContext.");

            // Truy vấn các TourScheduleTicket của Tour có TourSchedule.TourId == request.TourId
            var query = _context.TourScheduleTicket
                .Include(ts => ts.TourSchedule)
                .Where(ts => ts.TourSchedule.TourId == request.TourId);

            // Nếu có gửi ngày, lọc theo khoảng thời gian (dựa trên StartDate của TourSchedule)
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value.Date;
                var end = request.EndDate.Value.Date;
                query = query.Where(ts => ts.TourSchedule.StartDate >= start && ts.TourSchedule.StartDate <= end);
            }

            var scheduleTickets = await query.ToListAsync(cancellationToken);
            if (!scheduleTickets.Any())
            {
                return ApiResponse<string>.Failure("No schedule tickets found for the specified criteria", 404);
            }

            // Cập nhật các thuộc tính của từng TourScheduleTicket
            foreach (var ticket in scheduleTickets)
            {
                if (request.NewNetCost.HasValue)
                {
                    dbContext.Entry(ticket).Property("NetCost").CurrentValue = request.NewNetCost.Value;
                }
                if (request.NewAvailableTicket.HasValue)
                {
                    dbContext.Entry(ticket).Property("AvailableTicket").CurrentValue = request.NewAvailableTicket.Value;
                }
            }

            // Nếu không gửi ngày, cập nhật luôn giá mặc định (DefaultNetCost) cho tất cả các TicketType của Tour
            if (!request.StartDate.HasValue && !request.EndDate.HasValue && request.NewNetCost.HasValue)
            {
                var ticketTypes = await _context.TicketTypes
                    .Where(tt => tt.TourId == request.TourId)
                    .ToListAsync(cancellationToken);

                foreach (var tt in ticketTypes)
                {
                    var scheduleTicketsForType = scheduleTickets.Where(st => st.TicketTypeId == tt.Id);
                    foreach (var st in scheduleTicketsForType)
                    {
                        dbContext.Entry(st).Property("NetCost").CurrentValue = request.NewNetCost.Value;
                    }
                    dbContext.Entry(tt).Property("DefaultNetCost").CurrentValue = request.NewNetCost.Value;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Ticket schedules updated successfully", "Update successful");
        }
    }
}
