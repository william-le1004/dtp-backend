using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Feedback.Commands
{
    public record FeedbackResponse(
        Guid Id,
        Guid TourScheduleId,
        string? Description
    );

    public record CreateFeedbackCommand(
        Guid TourScheduleId,
        string? Description
    ) : IRequest<ApiResponse<FeedbackResponse>>;

    public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, ApiResponse<FeedbackResponse>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateFeedbackHandler(IDtpDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<ApiResponse<FeedbackResponse>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy userId từ context
            var userId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return ApiResponse<FeedbackResponse>.Failure("User is not authenticated", 401);

            // 2. Kiểm tra TourSchedule có tồn tại chưa
            bool scheduleExists = await _context.TourSchedules
                .AsNoTracking()
                .AnyAsync(ts => ts.Id == request.TourScheduleId, cancellationToken);
            if (!scheduleExists)
                return ApiResponse<FeedbackResponse>.Failure("TourSchedule not found", 404);

            // 3. Kiểm tra User có tồn tại chưa (AspNetUsers table)
            bool userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == userId, cancellationToken);
            if (!userExists)
                return ApiResponse<FeedbackResponse>.Failure("User not found", 404);

            // 4. Kiểm tra xem user đã feedback cho tour schedule này chưa
            var existingFeedback = await _context.Feedbacks
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.TourScheduleId == request.TourScheduleId, cancellationToken);

            if (existingFeedback != null)
                return ApiResponse<FeedbackResponse>.Failure("You have already provided feedback for this tour schedule", 400);

            // 5. Tạo Feedback
            var feedback = new Domain.Entities.Feedback
            {
                Id = Guid.NewGuid(),                  // sinh khoá chính
                TourScheduleId = request.TourScheduleId,
                UserId = userId,
                Description = request.Description
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new FeedbackResponse(
                feedback.Id,
                feedback.TourScheduleId,
                feedback.Description
            );
            return ApiResponse<FeedbackResponse>.SuccessResult(response, "Feedback created successfully");
        }
    }
}
