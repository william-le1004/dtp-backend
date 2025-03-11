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
    // Query nhận vào TourId và trả về danh sách ngày mở (OpenDate)
    public record GetTourScheduleByTourIDQuery(Guid TourId) : IRequest<ApiResponse<List<DateTime>>>;

    public class GetTourScheduleByTourIDQueryHandler : IRequestHandler<GetTourScheduleByTourIDQuery, ApiResponse<List<DateTime>>>
    {
        private readonly IDtpDbContext _context;

        public GetTourScheduleByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<DateTime>>> Handle(GetTourScheduleByTourIDQuery request, CancellationToken cancellationToken)
        {
            // Truy vấn danh sách OpenDate từ bảng TourSchedule theo TourId, loại bỏ trùng lặp và sắp xếp theo thứ tự tăng dần
            var openDays = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId)
                .Select(ts => ts.OpenDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(cancellationToken);

            return ApiResponse<List<DateTime>>.SuccessResult(openDays, "Tour schedules open days retrieved successfully");
        }
    }
}
