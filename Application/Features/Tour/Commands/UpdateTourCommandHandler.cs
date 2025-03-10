using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
// Giả sử TourResponse được định nghĩa: record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);

namespace Application.Features.Tour.Commands
{
    // Command cập nhật thông tin cơ bản của Tour
    public record UpdateTourInforCommand(
        Guid TourId,
        string Title,
        Guid? CompanyId,
        Guid? Category,
        string? Description
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
            tour.Update(request.Title, request.CompanyId, request.Category, request.Description);

            // Cập nhật lại vào DbContext
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO response
            var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.CategoryId, tour.Description);
            return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour updated successfully");
        }
    }
}