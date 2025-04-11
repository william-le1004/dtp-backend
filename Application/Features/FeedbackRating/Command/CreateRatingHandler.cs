using Application.Common;
using Application.Contracts.Persistence;
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
        Guid TourId,
        string UserId,
        int Star,
        string Comment,
        List<string> Images
    );

    // Command tạo Rating; client gửi TourId, UserId, Star, Comment và danh sách Images (có thể null)
    public record CreateRatingCommand(
        Guid TourId,
        string UserId,
        int Star,
        string Comment,
        List<string>? Images
    ) : IRequest<ApiResponse<RatingResponse>>;

    public class CreateRatingHandler : IRequestHandler<CreateRatingCommand, ApiResponse<RatingResponse>>
    {
        private readonly IDtpDbContext _context;

        public CreateRatingHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<RatingResponse>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            // Tạo một entity Rating mới với các dữ liệu từ command.
            var rating = new Domain.Entities.Rating
            {
                TourId = request.TourId,
                UserId = request.UserId,
                Star = request.Star,
                Comment = request.Comment,
                Images = request.Images ?? new List<string>()
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO trả về
            var responseDto = new RatingResponse(
                Id: rating.Id,
                TourId: rating.TourId,
                UserId: rating.UserId,
                Star: rating.Star,
                Comment: rating.Comment,
                Images: rating.Images
            );

            return ApiResponse<RatingResponse>.SuccessResult(responseDto, "Rating created successfully");
        }
    }
}
