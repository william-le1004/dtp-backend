using Application.Contracts.Persistence;
using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Tour.Commands
{
    // Command thêm lịch trình cho Tour theo khoảng thời gian và tần suất
    public record AddScheduleCommand(
        Guid TourId,
        DateTime OpenDay,
        DateTime CloseDay,
        string ScheduleFrequency  // "daily", "weekly", "monthly"
    ) : IRequest<ApiResponse<string>>;

    public class AddScheduleHandler : IRequestHandler<AddScheduleCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public AddScheduleHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
        {
            // Lấy Tour theo TourId, bao gồm các Ticket (TicketType) đã có
            var tour = await _context.Tours
                .Include(t => t.Tickets)
                .Include(t => t.TourSchedules)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

            if (tour == null)
            {
                return ApiResponse<string>.Failure("Tour not found", 404);
            }

            // Cast _context sang DbContext để sử dụng Entry cho các thuộc tính private
            var dbContext = _context as DbContext;
            if (dbContext == null)
            {
                throw new Exception("The IDtpDbContext instance is not a DbContext. Ensure your context implements Microsoft.EntityFrameworkCore.DbContext.");
            }

            // Xác định hàm tăng theo tần suất: daily (1 ngày), weekly (7 ngày), monthly (1 tháng)
            DateTime currentDay = request.OpenDay.Date;
            Func<DateTime, DateTime> stepFunc = request.ScheduleFrequency.ToLower() switch
            {
                "weekly" => d => d.AddDays(7),
                "monthly" => d => d.AddMonths(1),
                _ => d => d.AddDays(1)  // Mặc định là daily
            };

            // Lặp qua từng ngày (hoặc tuần, hoặc tháng) trong khoảng thời gian [OpenDay, CloseDay]
            while (currentDay <= request.CloseDay.Date)
            {
                // Tạo mới TourSchedule
                var schedule = new TourSchedule();
                dbContext.Entry(schedule).Property("TourId").CurrentValue = tour.Id;
                dbContext.Entry(schedule).Property("OpenDate").CurrentValue = currentDay;
                dbContext.Entry(schedule).Property("CloseDate").CurrentValue = currentDay;
                // Không cần cập nhật PriceChangeRate hay Remark theo yêu cầu

                // Với mỗi TicketType của Tour, tạo vé lịch trình (TourScheduleTicket) với giá mặc định của TicketType
                foreach (var ticketType in tour.Tickets)
                {
                    // Giả sử số lượng vé mặc định là 100
                    var scheduleTicket = new TourScheduleTicket(
                        ticketType.DefaultNetCost,
                        100,
                        ticketType.Id,
                        schedule.Id
                    );
                    schedule.AddTicket(scheduleTicket);
                }

                // Thêm lịch trình vào collection của Tour
                tour.TourSchedules.Add(schedule);

                // Tăng currentDay theo tần suất đã chỉ định
                currentDay = stepFunc(currentDay);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour schedules added successfully", "Schedule added successfully");
        }
    }
}
