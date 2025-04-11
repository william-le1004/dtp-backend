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
    // DTO cho Rating, bao gồm thông tin rating và thông tin của User
    public record RatingDto(
        Guid Id,
        Guid TourId,
        string UserId,
        string UserName,
        string UserEmail,
        int Star,
        string Comment,
        List<string>? Images = null
    );

    // Query: nhận TourId và trả về danh sách RatingDto được bọc trong ApiResponse
    public record GetListRatingByTourQuery(Guid TourId) : IRequest<ApiResponse<List<RatingDto>>>;

    public class GetListRatingByTourQueryHandler : IRequestHandler<GetListRatingByTourQuery, ApiResponse<List<RatingDto>>>
    {
        private readonly IDtpDbContext _context;

        public GetListRatingByTourQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<RatingDto>>> Handle(GetListRatingByTourQuery request, CancellationToken cancellationToken)
        {
            // Lấy danh sách Rating cho Tour theo TourId, include thông tin User để lấy tên và email
            var ratings = await _context.Ratings
                .Include(r => r.User)
                .Where(r => r.TourId == request.TourId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Chuyển đổi sang DTO
            var ratingDtos = ratings.Select(r => new RatingDto(
                Id: r.Id,
                TourId: r.TourId,
                UserId: r.UserId,
                UserName: r.User.Name,
                UserEmail: r.User.Email,
                Star: r.Star,
                Comment: r.Comment
            )).ToList();

            return ApiResponse<List<RatingDto>>.SuccessResult(ratingDtos, "Ratings retrieved successfully");
        }
    }
}
