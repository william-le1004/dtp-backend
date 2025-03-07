using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // Command nhận các thông tin cần thiết để xóa lịch trình của một Tour
    public record DeleteTourScheduleCommand(
        Guid TourId,
        DateTime OpenDay,
        DateTime CloseDay
    ) : IRequest<ApiResponse<string>>;

    public class DeleteTourScheduleHandler : IRequestHandler<DeleteTourScheduleCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public DeleteTourScheduleHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(DeleteTourScheduleCommand request, CancellationToken cancellationToken)
        {
            // Lấy tất cả các TourSchedule của Tour có TourId = request.TourId
            // và có StartDate và EndDate nằm trong khoảng [OpenDay, CloseDay]
            var schedules = await _context.TourSchedules
                .Where(s => s.TourId == request.TourId &&
                            s.OpenDate >= request.OpenDay.Date &&
                            s.CloseDate <= request.CloseDay.Date)
                .Include(s => s.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
            {
                return ApiResponse<string>.Failure("No tour schedules found in the given date range for the specified Tour", 404);
            }

            // Xóa các vé lịch trình (TourScheduleTicket) của từng schedule nếu không có cascade delete
            foreach (var schedule in schedules)
            {
                foreach (var ticket in schedule.TourScheduleTickets.ToList())
                {
                    _context.TourScheduleTicket.Remove(ticket);
                }
                _context.TourSchedules.Remove(schedule);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour schedules and associated tickets deleted successfully", "Deletion successful");
        }
    }
}
