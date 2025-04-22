using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
    // Query nhận vào TourId và trả về danh sách FeedbackResponse được bọc trong ApiResponse
    public record GetListFeedbackByTourQuery(Guid TourId) : IRequest<ApiResponse<List<FeedbackResponse>>>;

    // DTO FeedbackResponse mở rộng thêm thông tin người dùng
    public record FeedbackResponse(
        Guid Id,
        Guid TourScheduleId,
        string UserId,
        string UserName,
        string UserEmail,
        string? Description
    );

    public class GetListFeedbackByTourQueryHandler : IRequestHandler<GetListFeedbackByTourQuery, ApiResponse<List<FeedbackResponse>>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetListFeedbackByTourQueryHandler(IDtpDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<ApiResponse<List<FeedbackResponse>>> Handle(GetListFeedbackByTourQuery request, CancellationToken cancellationToken)
        {
            // Base query with includes
            var query = _context.Feedbacks
                .Include(f => f.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .Include(f => f.User)
                .Where(f => f.TourSchedule.TourId == request.TourId);

            // Filter based on user role
            if (_userContextService.IsOperatorRole())
            {
                var companyId = _userContextService.GetCompanyId();
                if (!companyId.HasValue)
                {
                    return ApiResponse<List<FeedbackResponse>>.Failure("Operator is not associated with any company", 403);
                }

                // Operator can only see feedbacks for tours belonging to their company
                query = query.Where(f => f.TourSchedule.Tour.CompanyId == companyId.Value);
            }
            // Admin can see all feedbacks, so no additional filter needed

            var feedbacks = await query.ToListAsync(cancellationToken);

            var feedbackDtos = feedbacks.Select(f => new FeedbackResponse(
                Id: f.Id,
                TourScheduleId: f.TourScheduleId,
                UserId: f.UserId,
                UserName: f.User.Name,        
                UserEmail: f.User.Email,      
                Description: f.Description
            )).ToList();

            return ApiResponse<List<FeedbackResponse>>.SuccessResult(feedbackDtos, "Feedbacks retrieved successfully");
        }
    }
}
