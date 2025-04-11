using Application.Common;
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

        public GetListFeedbackByTourQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<FeedbackResponse>>> Handle(GetListFeedbackByTourQuery request, CancellationToken cancellationToken)
        {
            // Include thông tin TourSchedule và User để truy cập TourSchedule.TourId và thông tin người dùng
            var feedbacks = await _context.Feedbacks
                .Include(f => f.TourSchedule)
                .Include(f => f.User)
                .Where(f => f.TourSchedule.TourId == request.TourId)
                .ToListAsync(cancellationToken);

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
