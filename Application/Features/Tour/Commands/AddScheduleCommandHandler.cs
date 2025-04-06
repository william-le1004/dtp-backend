using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Commands
{
    // Command thêm lịch trình cho Tour theo khoảng thời gian và tần suất
    public record AddScheduleCommand(
        Guid TourId,
        DateTime OpenDay,
        DateTime CloseDay,
        string ScheduleFrequency // "daily", "weekly", "monthly"
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
            // Lấy Tour theo TourId, bao gồm TicketType và lịch trình hiện có
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

            // Xác định hàm bước theo tần suất: daily (1 ngày), weekly (7 ngày), monthly (1 tháng)
            DateTime currentDay = request.OpenDay.Date;
            Func<DateTime, DateTime> stepFunc = request.ScheduleFrequency.ToLower() switch
            {
                "weekly" => d => d.AddDays(7),
                "monthly" => d => d.AddMonths(1),
                _ => d => d.AddDays(1) // default là daily
            };

            while (currentDay <= request.CloseDay.Date)
            {
                // Kiểm tra nếu trong tour đã có schedule cho ngày currentDay (với IsDeleted == false)
                bool exists = tour.TourSchedules.Any(s => s.OpenDate.Date == currentDay && !s.IsDeleted);
                if (!exists)
                {
                    var schedule = new TourSchedule();
                    // Cập nhật các thuộc tính private của schedule thông qua dbContext.Entry
                    dbContext.Entry(schedule).Property("TourId").CurrentValue = tour.Id;
                    dbContext.Entry(schedule).Property("OpenDate").CurrentValue = currentDay;
                    dbContext.Entry(schedule).Property("CloseDate").CurrentValue = currentDay;
                    // Không cần cập nhật PriceChangeRate hay Remark theo yêu cầu

                    // Với mỗi TicketType của Tour, tạo vé lịch trình (TourScheduleTicket) với giá mặc định của TicketType
                    foreach (var ticketType in tour.Tickets)
                    {
                        // Giả sử số lượng vé mặc định là 100
                        var scheduleTicket = new TourScheduleTicket(ticketType.DefaultNetCost, 100, ticketType.Id, schedule.Id);
                        schedule.AddTicket(scheduleTicket);
                    }

                    tour.TourSchedules.Add(schedule);
                }
                // Nếu đã có schedule cho ngày này, ta bỏ qua và không tạo mới.

                currentDay = stepFunc(currentDay);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour schedules added successfully", "Schedule added successfully");
        }

    }
}