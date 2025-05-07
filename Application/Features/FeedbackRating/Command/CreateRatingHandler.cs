using Application.Common;
using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.DataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Rating.Commands
{
    // DTO trả về thông tin Rating
    public record RatingResponse(
        Guid Id,
        Guid BookingId,
        Guid TourId,
        int Star,
        string Comment       
    );

    // Command tạo Rating; client gửi TourId, UserId, Star, Comment và danh sách Images (có thể null)
    public record CreateRatingCommand(
        Guid TourId,
        Guid BookingId,
        int Star,
        string Comment,
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

            var existingRating = await _context.Ratings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == userId && r.TourId == request.TourId, cancellationToken);

            if (existingRating != null)
                return ApiResponse<RatingResponse>.Failure("You have already rated this tour", 400);

            var rating = new Domain.Entities.Rating
            {
                Id = Guid.NewGuid(), 
                TourId = request.TourId,
                UserId = userId,
                ToourBookingId = request.BookingId,
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
            if (booking.Status != Domain.Enum.BookingStatus.Paid)
                booking.Complete();
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO trả về
            var responseDto = new RatingResponse(
                Id: rating.Id,
                TourId: rating.TourId,
                BookingId: rating.ToourBookingId,
                Star: rating.Star,
                Comment: rating.Comment
                );

            return ApiResponse<RatingResponse>.SuccessResult(responseDto, "Rating created successfully");
        }
    }
}
