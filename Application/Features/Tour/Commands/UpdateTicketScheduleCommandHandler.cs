using Application.Common;
using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    public record TicketKindUpdateInfo(
        TicketKind TicketKind,
        decimal? NewNetCost,
        int? NewAvailableTicket
    );

    public record UpdateTicketScheduleByTicketKindCommand(
        Guid TourId,
        List<TicketKindUpdateInfo> TicketKindUpdates,
        DateTime? StartDate,
        DateTime? EndDate
    ) : IRequest<ApiResponse<string>>;

    public class UpdateTicketScheduleByTicketKindHandler : IRequestHandler<UpdateTicketScheduleByTicketKindCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTicketScheduleByTicketKindHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(UpdateTicketScheduleByTicketKindCommand request, CancellationToken cancellationToken)
        {
            // Ép _context sang DbContext để sử dụng Entry
            var dbContext = _context as DbContext;
            if (dbContext == null)
                throw new Exception("The IDtpDbContext instance is not a DbContext.");

            // Lấy các TourScheduleTicket thuộc các TourSchedule có TourId bằng request.TourId
            var query = _context.TourScheduleTicket
                .Include(tst => tst.TourSchedule)
                .Where(tst => tst.TourSchedule.TourId == request.TourId);
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value.Date;
                var end = request.EndDate.Value.Date;
                query = query.Where(tst => tst.TourSchedule.OpenDate >= start && tst.TourSchedule.OpenDate <= end);
            }
            var scheduleTickets = await query.ToListAsync(cancellationToken);
            if (!scheduleTickets.Any())
            {
                return ApiResponse<string>.Failure("No schedule tickets found for the specified criteria", 404);
            }

            // Tải các TicketType của tour để tra cứu theo TicketKind
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.TourId == request.TourId)
                .ToListAsync(cancellationToken);

            // Với mỗi thông tin cập nhật của loại vé trong command, cập nhật các vé lịch trình tương ứng
            foreach (var updateInfo in request.TicketKindUpdates)
            {
                // Lấy danh sách TicketType IDs có TicketKind trùng với updateInfo
                var matchingTicketTypeIds = ticketTypes
                    .Where(tt => tt.TicketKind == updateInfo.TicketKind)
                    .Select(tt => tt.Id)
                    .Distinct()
                    .ToList();

                // Đối với mỗi vé lịch trình có TicketTypeId phù hợp, cập nhật giá và số lượng theo updateInfo
                foreach (var scheduleTicket in scheduleTickets.Where(st => matchingTicketTypeIds.Contains(st.TicketTypeId)))
                {
                    if (updateInfo.NewNetCost.HasValue)
                    {
                        dbContext.Entry(scheduleTicket).Property("NetCost").CurrentValue = updateInfo.NewNetCost.Value;
                    }
                    if (updateInfo.NewAvailableTicket.HasValue)
                    {
                        dbContext.Entry(scheduleTicket).Property("AvailableTicket").CurrentValue = updateInfo.NewAvailableTicket.Value;
                    }
                }

                // Cập nhật mặc định giá (DefaultNetCost) cho các TicketType của tour có TicketKind tương ứng
                foreach (var ticketType in ticketTypes.Where(tt => tt.TicketKind == updateInfo.TicketKind))
                {
                    if (updateInfo.NewNetCost.HasValue)
                    {
                        dbContext.Entry(ticketType).Property("DefaultNetCost").CurrentValue = updateInfo.NewNetCost.Value;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Ticket schedules updated successfully", "Update successful");
        }
    }
}
