using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.DataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rating.Commands
{
    public record RatingResponse(
        Guid Id,
        Guid BookingId,
        Guid TourId,
        int Star,
        string Comment
    );
    
    public record CreateRatingCommand(
        Guid TourId,
        Guid BookingId,
        int Star,
        string Comment,
        Guid? TourScheduleId,
        List<string>? Images = null
    ) : IRequest<ApiResponse<RatingResponse>>;

    public class CreateRatingHandler : IRequestHandler<CreateRatingCommand, ApiResponse<RatingResponse>>
    {
        private readonly IDtpDbContext _context;
        private readonly IUserContextService _userContextService;

        public CreateRatingHandler(IDtpDbContext context, IUserContextService userContext )
        {
            _context = context;
            _userContextService = userContext;
        }

        public async Task<ApiResponse<RatingResponse>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return ApiResponse<RatingResponse>.Failure("User is not authenticated", 401);

            var existingRating =  _context.Ratings.Any(rating => rating.TourBookingId == request.BookingId);

            if (existingRating)
                return ApiResponse<RatingResponse>.Failure("You have already rated this tour", 400);

            var rating = new Domain.Entities.Rating
            {
                Id = Guid.NewGuid(), 
                TourId = request.TourId,
                UserId = userId,
                TourBookingId = request.BookingId,
                Star = request.Star,
                Comment = request.Comment,
            };
            foreach (var image in request.Images ?? new List<string>())
            {
                _context.ImageUrls.Add(new ImageUrl(rating.Id,image));
            }
            _context.Ratings.Add(rating);
            var booking = await _context.TourBookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);
            if (!booking.IsCompleted())
            {
                booking.ToCompleted();
            }
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO trả về
            var responseDto = new RatingResponse(
                Id: rating.Id,
                TourId: rating.TourId,
                BookingId: rating.TourBookingId,
                Star: rating.Star,
                Comment: rating.Comment
                );

            return ApiResponse<RatingResponse>.SuccessResult(responseDto, "Rating created successfully");
        }
    }
}