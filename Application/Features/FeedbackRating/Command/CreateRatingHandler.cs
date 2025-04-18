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
        Guid TourId,
        int Star,
        string Comment       
    );

    // Command tạo Rating; client gửi TourId, UserId, Star, Comment và danh sách Images (có thể null)
    public record CreateRatingCommand(
        Guid TourId,
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
            // Lấy UserId từ dịch vụ người dùng hiện tại
            var userId = _userContextService.GetCurrentUserId();
            // Tạo một entity Rating mới với các dữ liệu từ command.
            var rating = new Domain.Entities.Rating
            {
                TourId = request.TourId,
                UserId = userId,
                Star = request.Star,
                Comment = request.Comment,
            };
            // Nếu có danh sách ảnh, gán vào Rating
            foreach (var image in request.Images ?? new List<string>())
            {
                _context.ImageUrls.Add(new ImageUrl(rating.Id,image));
            }
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO trả về
            var responseDto = new RatingResponse(
                Id: rating.Id,
                TourId: rating.TourId,
                Star: rating.Star,
                Comment: rating.Comment
                );

            return ApiResponse<RatingResponse>.SuccessResult(responseDto, "Rating created successfully");
        }
    }
}
