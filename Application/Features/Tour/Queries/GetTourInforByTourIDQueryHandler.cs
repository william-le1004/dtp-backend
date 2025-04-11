using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
    public record TourInforDto(
        Guid TourId,
        string Title,
        string? Description,
        Guid? Category,
        string? About,
        string? Include,
        string? Pickinfor,
        string? Img
    );
    // Query nhận vào TourId
    public record GetTourInforByTourIDQuery(Guid TourId) : IRequest<ApiResponse<TourInforDto>>;

    public class GetTourInforByTourIDQueryHandler : IRequestHandler<GetTourInforByTourIDQuery, ApiResponse<TourInforDto>>
    {
        private readonly IDtpDbContext _context;

        public GetTourInforByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<TourInforDto>> Handle(GetTourInforByTourIDQuery request, CancellationToken cancellationToken)
        {
            // Truy vấn Tour theo id, include Category để lấy tên danh mục
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

            if (tour == null)
            {
                return ApiResponse<TourInforDto>.Failure("Tour not found", 404);
            }

            // Tạo DTO trả về, không bao gồm thông tin Destination
            var dto = new TourInforDto(
                TourId: tour.Id,
                Title: tour.Title,
                Description: tour.Description,
                Category: tour.CategoryId,
                About: tour.About,
                Include: tour.Include,
                Pickinfor: tour.Pickinfor,
                Img: _context.ImageUrls.Any(i => i.RefId == tour.Id) ? _context.ImageUrls.FirstOrDefault(i => i.RefId == tour.Id).Url : null
            );

            return ApiResponse<TourInforDto>.SuccessResult(dto, "Tour information retrieved successfully");
        }
    }
}
