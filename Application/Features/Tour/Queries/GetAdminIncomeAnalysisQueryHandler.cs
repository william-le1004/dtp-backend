using Application.Common;
using Application.Contracts.Persistence;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
   

    // DTO tổng hợp kết quả phân tích cho Admin
    public record AdminIncomeAnalysisResponse(
        List<DailySalesDto> DailySales,
        List<TopTourDto> TopTours,
        List<NewestBookingDto> NewestBookings
    );

    // Query không cần tham số vì admin phân tích toàn hệ thống
    public record GetAdminIncomeAnalysisQuery() : IRequest<ApiResponse<AdminIncomeAnalysisResponse>>;

    public class GetAdminIncomeAnalysisQueryHandler : IRequestHandler<GetAdminIncomeAnalysisQuery, ApiResponse<AdminIncomeAnalysisResponse>>
    {
        private readonly IDtpDbContext _context;

        public GetAdminIncomeAnalysisQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AdminIncomeAnalysisResponse>> Handle(GetAdminIncomeAnalysisQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Xác định tháng hiện tại
                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // 1. DailySales: lấy tất cả booking được tạo trong tháng hiện tại.
                var bookingsThisMonth = await _context.TourBookings
                    .Include(tb => tb.Tickets)
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .Where(tb => tb.CreatedAt >= firstDayOfMonth && tb.CreatedAt <= lastDayOfMonth)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var dailySales = bookingsThisMonth
                    .GroupBy(tb => DateOnly.FromDateTime(tb.CreatedAt))
                    .Select(g => new DailySalesDto(
                        Date: g.Key,
                        TicketsSold: g.Sum(tb => tb.Tickets?.Sum(t => t.Quantity) ?? 0),
                        TotalIncome: g.Sum(tb => tb.NetCost())
                    ))
                    .OrderBy(ds => ds.Date)
                    .ToList();

                // 2. TopTours: lấy tất cả booking của toàn hệ thống, nhóm theo TourId
                var allBookings = await _context.TourBookings
                    .Include(tb => tb.Tickets)
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var topToursGroup = allBookings
                    .Where(tb => tb.TourSchedule?.Tour != null)
                    .GroupBy(tb => tb.TourSchedule.TourId)
                    .Select(g => new
                    {
                        TourId = g.Key,
                        TicketsSold = g.Sum(tb => tb.Tickets?.Sum(t => t.Quantity) ?? 0)
                    })
                    .OrderByDescending(x => x.TicketsSold)
                    .Take(5)
                    .ToList();

                var topTours = new List<TopTourDto>();
                foreach (var group in topToursGroup)
                {
                    var tour = await _context.Tours
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == group.TourId, cancellationToken);
                    if (tour != null)
                    {
                        topTours.Add(new TopTourDto(
                            TourId: tour.Id,
                            TourTitle: tour.Title,
                            TicketsSold: group.TicketsSold
                        ));
                    }
                }

                // 3. NewestBookings: 5 booking mới nhất
                var newestBookingsList = await _context.TourBookings
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .Where(tb => tb.TourSchedule != null && tb.TourSchedule.Tour != null)
                    .AsNoTracking()
                    .OrderByDescending(tb => tb.CreatedAt)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var newestBookings = newestBookingsList.Select(tb => new NewestBookingDto(
                    BookingId: tb.Id,
                    BookingCode: tb.Code,
                    CreatedAt: tb.CreatedAt,
                    TourTitle: tb.TourSchedule?.Tour?.Title ?? "Unknown Tour",
                    TotalCost: tb.NetCost()
                )).ToList();

                var responseDto = new AdminIncomeAnalysisResponse(
                    DailySales: dailySales,
                    TopTours: topTours,
                    NewestBookings: newestBookings
                );

                return ApiResponse<AdminIncomeAnalysisResponse>.SuccessResult(responseDto, "Admin income analysis retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AdminIncomeAnalysisResponse>.Failure($"Error retrieving admin income analysis: {ex.Message}", 500);
            }
        }
    }
}
