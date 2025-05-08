using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries
{
    public record Schedule(
        DateOnly OpenDate,
        string Status
    );

    public record GetTourScheduleByTourIDQuery(Guid TourId)
        : IRequest<ApiResponse<List<Schedule>>>;

    public class GetTourScheduleByTourIDQueryHandler
        : IRequestHandler<GetTourScheduleByTourIDQuery, ApiResponse<List<Schedule>>>
    {
        private readonly IDtpDbContext _context;

        public GetTourScheduleByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<Schedule>>> Handle(
     GetTourScheduleByTourIDQuery request,
     CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            // 1) Lấy tất cả TourSchedule có OpenDate != null
            var tourSchedules = await _context.TourSchedules
                .AsNoTracking()
                .Where(ts =>
                    ts.TourId == request.TourId
                    && ts.OpenDate.HasValue)
                .ToListAsync(cancellationToken);

            if (!tourSchedules.Any())
                return ApiResponse<List<Schedule>>.Failure("Không tìm thấy lịch trình cho Tour này", 404);

            var now = DateTime.Now;

            // 2) Map thành danh sách Schedule, chưa dedupe
            var all = tourSchedules
                .Select(ts =>
                {
                    var open = DateOnly.FromDateTime(ts.OpenDate!.Value);

                    string status;
                    if (ts.IsCanceled())
                        status = "cancel";
                    else if (ts.IsCompleted())
                        status = "complete";
                    else
                        status = "upcoming";

                    return new Schedule(open, status);
                })
                .ToList();

            // 3) Dedupe theo OpenDate, ưu tiên upcoming > complete > cancel
            var priority = new Dictionary<string, int>
            {
                ["upcoming"] = 0,
                ["complete"] = 1,
                ["cancel"] = 2
            };

            var result = all
                .GroupBy(s => s.OpenDate)
                .Select(g =>
                    g
                    .OrderBy(s => priority.GetValueOrDefault(s.Status, 99))
                    .First()
                )
                .OrderBy(s => s.OpenDate)
                .ToList();

            return ApiResponse<List<Schedule>>
                .SuccessResult(result, "Lịch trình Tour được tải thành công");
        }

    }
}
