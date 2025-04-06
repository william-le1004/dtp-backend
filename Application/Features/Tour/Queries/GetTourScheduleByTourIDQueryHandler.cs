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
    public record GetTourScheduleByTourIDQuery(Guid TourId) : IRequest<ApiResponse<List<DateOnly>>>;

    public class GetTourScheduleByTourIDQueryHandler : IRequestHandler<GetTourScheduleByTourIDQuery, ApiResponse<List<DateOnly>>>
    {
        private readonly IDtpDbContext _context;

        public GetTourScheduleByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<DateOnly>>> Handle(GetTourScheduleByTourIDQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var openDaysDateTime = await _context.TourSchedules
                .Where(ts => ts.TourId == request.TourId
                             && !ts.IsDeleted
                             && ts.OpenDate >= today)
                .GroupBy(ts => new { ts.OpenDate.Year, ts.OpenDate.Month, ts.OpenDate.Day })
                .Select(g => new DateTime(g.Key.Year, g.Key.Month, g.Key.Day))
                .OrderBy(d => d)
                .ToListAsync(cancellationToken);

            var openDays = openDaysDateTime.Select(d => DateOnly.FromDateTime(d)).ToList();

            return ApiResponse<List<DateOnly>>.SuccessResult(openDays, "Tour schedules open days retrieved successfully");
        }


    }
}
