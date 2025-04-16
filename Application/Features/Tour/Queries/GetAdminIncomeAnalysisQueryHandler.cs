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
    // DTO cho dữ liệu biểu đồ theo ngày
    public record DailySalesDto(
        DateOnly Date,
        int TicketsSold,
        decimal TotalIncome
    );

    // DTO cho top tour: gồm TourId, TourTitle và số vé bán ra
    public record TopTourDto(
        Guid TourId,
        string TourTitle,
        int TicketsSold
    );

    // DTO cho booking mới nhất: gồm BookingId, BookingCode, ngày tạo, tên tour, tổng chi phí (NetCost)
    public record NewestBookingDto(
        Guid BookingId,
        string BookingCode,
        DateTime CreatedAt,
        string TourTitle,
        decimal TotalCost
    );

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
            // Xác định tháng hiện tại
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // 1. DailySales: lấy tất cả booking được tạo trong tháng hiện tại.
            // Giả sử mỗi TourBooking có CreatedAt và phương thức NetCost() trả về tổng chi phí của booking.
            var bookingsThisMonth = await _context.TourBookings
                .Include(tb => tb.Tickets) // Tickets chứa Quantity
                .Include(tb => tb.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .Where(tb => tb.CreatedAt >= firstDayOfMonth && tb.CreatedAt <= lastDayOfMonth)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dailySales = bookingsThisMonth
                .GroupBy(tb => DateOnly.FromDateTime(tb.CreatedAt))
                .Select(g => new DailySalesDto(
                    Date: g.Key,
                    TicketsSold: g.Sum(tb => tb.Tickets.Sum(t => t.Quantity)),
                    TotalIncome: g.Sum(tb => tb.NetCost())
                ))
                .OrderBy(ds => ds.Date)
                .ToList();

            // 2. TopTours: lấy tất cả booking của toàn hệ thống, nhóm theo TourId (qua TourSchedule)
            var allBookings = await _context.TourBookings
                .Include(tb => tb.Tickets)
                .Include(tb => tb.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var topToursGroup = allBookings
                .GroupBy(tb => tb.TourSchedule.TourId)
                .Select(g => new
                {
                    TourId = g.Key,
                    TicketsSold = g.Sum(tb => tb.Tickets.Sum(t => t.Quantity))
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

            // 3. NewestBookings: 5 booking mới nhất (theo CreatedAt) trên toàn hệ thống
            var newestBookingsList = await _context.TourBookings
                .Include(tb => tb.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .AsNoTracking()
                .OrderByDescending(tb => tb.CreatedAt)
                .Take(5)
                .ToListAsync(cancellationToken);
            var newestBookings = newestBookingsList.Select(tb => new NewestBookingDto(
                BookingId: tb.Id,
                BookingCode: tb.Code,
                CreatedAt: tb.CreatedAt,
                TourTitle: tb.TourSchedule.Tour.Title,
                TotalCost: tb.NetCost()
            )).ToList();

            var responseDto = new AdminIncomeAnalysisResponse(
                DailySales: dailySales,
                TopTours: topTours,
                NewestBookings: newestBookings
            );

            return ApiResponse<AdminIncomeAnalysisResponse>.SuccessResult(responseDto, "Admin income analysis retrieved successfully");
        }
    }
}
