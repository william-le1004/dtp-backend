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
    // Command nhận vào TourId và khoảng thời gian cần xóa lịch trình
    public record DeleteTourSchedule(
        DateOnly StartDay,
        DateOnly EndDay
    );
    public record DeleteTourScheduleCommand(Guid TourId, DateOnly StartDay, DateOnly EndDay) : IRequest<ApiResponse<string>>;
    public class DeleteTourScheduleHandler : IRequestHandler<DeleteTourScheduleCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public DeleteTourScheduleHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(DeleteTourScheduleCommand request, CancellationToken cancellationToken)
        {
            // Lấy tất cả các TourSchedule của Tour có TourId = request.TourId và nằm trong khoảng thời gian được chỉ định
            var schedules = await _context.TourSchedules
                .Where(s => s.TourId == request.TourId &&
                            s.OpenDate.Date >= request.StartDay.ToDateTime(TimeOnly.MinValue) &&
                            s.CloseDate.Date <= request.EndDay.ToDateTime(TimeOnly.MinValue))
                .Include(s => s.TourScheduleTickets)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
            {
                return ApiResponse<string>.Failure("No tour schedules found in the given date range for the specified Tour", 404);
            }

            // Xóa các vé lịch trình (TourScheduleTicket) của từng schedule (nếu không có cascade delete)
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
