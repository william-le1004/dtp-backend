using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.Tour.Queries
{
    // DTO cho dữ liệu biểu đồ số vé bán ra và doanh thu theo ngày
    public record DailySalesDto(
        DateOnly Date,
        int TicketsSold,
        decimal TotalIncome
    );

    // DTO cho thông tin top tour: id, tiêu đề và tổng số vé đã bán
    public record TopTourDto(
        Guid TourId,
        string TourTitle,
        int TicketsSold
    );

    // DTO cho 5 booking mới nhất: id, mã đặt, ngày đặt, tên tour và tổng chi phí
    public record NewestBookingDto(
        Guid BookingId,
        string BookingCode,
        DateTime CreatedAt,
        string TourTitle,
        decimal TotalCost
    );

    // DTO tổng hợp kết quả analysis
    public record OperatorIncomeAnalysisResponse(
        List<DailySalesDto> DailySales,
        List<TopTourDto> TopTours,
        List<NewestBookingDto> NewestBookings
    );

    // Query: không cần tham số, vì CompanyId sẽ được lấy từ context (IUserContextService)
    public record GetOperatorIncomeAnalysisQuery() : IRequest<ApiResponse<OperatorIncomeAnalysisResponse>>;

    public class GetOperatorIncomeAnalysisQueryHandler : IRequestHandler<GetOperatorIncomeAnalysisQuery, ApiResponse<OperatorIncomeAnalysisResponse>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetOperatorIncomeAnalysisQueryHandler(IDtpDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<ApiResponse<OperatorIncomeAnalysisResponse>> Handle(GetOperatorIncomeAnalysisQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy CompanyId của operator từ dịch vụ ngữ cảnh
                var companyId = _userContextService.GetCompanyId();
                if (!companyId.HasValue || companyId.Value == Guid.Empty)
                    return ApiResponse<OperatorIncomeAnalysisResponse>.Failure("Operator is not associated with any company", 404);

                // Xác định tháng hiện tại
                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // 1. Tính DailySales
                var bookingsThisMonth = await _context.TourBookings
                    .Include(tb => tb.Tickets)
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .Where(tb => tb.TourSchedule != null && 
                                tb.TourSchedule.Tour != null && 
                                tb.TourSchedule.Tour.CompanyId == companyId.Value &&
                                tb.CreatedAt >= firstDayOfMonth &&
                                tb.CreatedAt <= lastDayOfMonth)
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

                // 2. Top 5 Tours được mua nhiều nhất
                var tourSales = await _context.TourBookings
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .Include(tb => tb.Tickets)
                    .Where(tb => tb.TourSchedule != null && 
                                tb.TourSchedule.Tour != null && 
                                tb.TourSchedule.Tour.CompanyId == companyId.Value)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var topTourSales = tourSales
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
                foreach (var sale in topTourSales)
                {
                    var tour = await _context.Tours
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == sale.TourId, cancellationToken);
                    if (tour != null)
                    {
                        topTours.Add(new TopTourDto(
                            TourId: tour.Id,
                            TourTitle: tour.Title,
                            TicketsSold: sale.TicketsSold
                        ));
                    }
                }

                // 3. 5 Newest Bookings
                var newestBookings = await _context.TourBookings
                    .Include(tb => tb.TourSchedule)
                        .ThenInclude(ts => ts.Tour)
                    .Where(tb => tb.TourSchedule != null && 
                                tb.TourSchedule.Tour != null && 
                                tb.TourSchedule.Tour.CompanyId == companyId.Value)
                    .AsNoTracking()
                    .OrderByDescending(tb => tb.CreatedAt)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var newestBookingDtos = newestBookings.Select(tb => new NewestBookingDto(
                    BookingId: tb.Id,
                    BookingCode: tb.Code,
                    CreatedAt: tb.CreatedAt,
                    TourTitle: tb.TourSchedule?.Tour?.Title ?? "Unknown Tour",
                    TotalCost: tb.NetCost()
                )).ToList();

                var responseDto = new OperatorIncomeAnalysisResponse(
                    DailySales: dailySales,
                    TopTours: topTours,
                    NewestBookings: newestBookingDtos
                );

                return ApiResponse<OperatorIncomeAnalysisResponse>.SuccessResult(responseDto, "Operator income analysis retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<OperatorIncomeAnalysisResponse>.Failure($"Error retrieving operator income analysis: {ex.Message}", 500);
            }
        }
    }
}
