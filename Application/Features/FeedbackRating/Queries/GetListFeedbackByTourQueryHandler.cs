using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries
{
    public record FeedbackResponse(
        Guid Id,
        Guid TourScheduleId,
        string UserId,
        string UserName,
        string UserEmail,
        string? Description,
        string TourTitle,
        DateTime OpenDate,
        string? CompanyName
    );

    // 1. Thêm record query không có TourId
    public record GetListFeedbackQuery() : IRequest<ApiResponse<List<FeedbackResponse>>>;

    // 2. Handler cho query này
    public class GetListFeedbackQueryHandler
        : IRequestHandler<GetListFeedbackQuery, ApiResponse<List<FeedbackResponse>>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetListFeedbackQueryHandler(
            IDtpDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<ApiResponse<List<FeedbackResponse>>> Handle(
    GetListFeedbackQuery request,
    CancellationToken cancellationToken)
        {
            // 1) Khai báo query là IQueryable<Feedback>
            IQueryable<Domain.Entities.Feedback> query =
                _context.Feedbacks
                    .AsNoTracking()
                    .Include(f => f.TourSchedule!)
                        .ThenInclude(ts => ts.Tour!)
                            .ThenInclude(t => t.Company!)
                    .Include(f => f.User!);

            // 2) Nếu là operator, thêm điều kiện lọc công ty
            if (_userContextService.IsOperatorRole())
            {
                var companyId = _userContextService.GetCompanyId();
                if (!companyId.HasValue)
                    return ApiResponse<List<FeedbackResponse>>
                        .Failure("Operator không gắn với công ty nào", 403);

                query = query.Where(f =>
                    f.TourSchedule!.Tour.CompanyId == companyId.Value);
            }

            // 3) Chạy truy vấn và map DTO
            var feedbacks = await query.ToListAsync(cancellationToken);
            var isOperator = _userContextService.IsOperatorRole();
            var result = feedbacks.Select(f => new FeedbackResponse(
                Id: f.Id,
                TourScheduleId: f.TourScheduleId,
                UserId: f.UserId,
                UserName: f.User!.Name,
                UserEmail: f.User.Email,
                Description: f.Description,
                TourTitle: f.TourSchedule!.Tour.Title,
                OpenDate: f.TourSchedule.OpenDate!.Value,
                CompanyName: isOperator ? null : f.TourSchedule.Tour.Company.Name
            )).ToList();

            return ApiResponse<List<FeedbackResponse>>
                .SuccessResult(result, "Feedbacks retrieved successfully");
        }

    }

}