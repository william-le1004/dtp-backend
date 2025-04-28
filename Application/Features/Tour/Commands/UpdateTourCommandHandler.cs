using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using Domain.DataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
// Giả sử TourResponse được định nghĩa: record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);

namespace Application.Features.Tour.Commands
{
    // Command cập nhật thông tin cơ bản của Tour
    public record UpdateTourInforCommand(
        Guid TourId,
        string Title,
        Guid? Category,
        string? Description,
        string? About,
        string? Include,
        string? Pickinfor,
        List<string>? img
    ) : IRequest<ApiResponse<TourResponse>>;

    public class UpdateTourInforHandler : IRequestHandler<UpdateTourInforCommand, ApiResponse<TourResponse>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTourInforHandler(IDtpDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<TourResponse>> Handle(UpdateTourInforCommand request,
            CancellationToken cancellationToken)
        {
            // Tìm Tour theo TourId
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
            
            if (tour == null)
            {
                return ApiResponse<TourResponse>.Failure("Tour not found", 404);
            }

            // Cập nhật thông tin của Tour
            tour.Update(request.Title, request.Category, request.Description,request.About,request.Include,request.Pickinfor);
            // Xóa các ImageUrls cũ và thêm mới
            var existingImageUrls = await _context.ImageUrls
                .Where(i => i.RefId == tour.Id)
                .ToListAsync(cancellationToken);
            _context.ImageUrls.RemoveRange(existingImageUrls);
            foreach (var img in request.img)
            {
                // Thêm các ảnh mới vào ImageUrls
                _context.ImageUrls.Add(new ImageUrl(tour.Id, img));
            }
            // Cập nhật lại vào DbContext
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO response
            var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.CategoryId, tour.Description,tour.About,tour.Include,tour.Pickinfor,tour.IsDeleted);
            return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour updated successfully");
        }
    }
}