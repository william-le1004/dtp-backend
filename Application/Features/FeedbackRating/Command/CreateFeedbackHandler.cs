using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Feedback.Commands
{
    // DTO FeedbackResponse: định nghĩa thông tin trả về sau khi tạo Feedback
    public record FeedbackResponse(
        Guid Id,
        Guid TourScheduleId,
        string? Description
    );

    // Command tạo Feedback; client gửi TourScheduleId, UserId và Description
    public record CreateFeedbackCommand(
        Guid TourScheduleId,
        string? Description
    ) : IRequest<ApiResponse<FeedbackResponse>>;

    // Handler xử lý tạo Feedback
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
            // Sử dụng fully qualified name để tránh nhầm lẫn với namespace Feedback
            var userId = _userContextService.GetCurrentUserId();
            var feedback = new Domain.Entities.Feedback
            {
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
