using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using Domain.DataModel;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
// Giả sử TourResponse được định nghĩa: record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);

namespace Application.Features.Tour.Commands
{
  
    public record UpdateTourDestinationCommand(
        Guid TourId,
        List<DestinationToAdd> Destinations
    ) : IRequest<ApiResponse<string >>;

    public class UpdateTourDestinationHandler : IRequestHandler<UpdateTourDestinationCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTourDestinationHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(UpdateTourDestinationCommand request,
            CancellationToken cancellationToken)
        {
            // Lấy Tour theo TourId kèm theo collection TourDestinations
            var tour = await _context.Tours
                .Include(t => t.TourDestinations)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
            if (tour == null)
            {
                return ApiResponse<string>.Failure("Tour not found", 404);
            }

            // Xóa hết các TourDestination hiện tại
            tour.TourDestinations.Clear();

            // Thêm mới danh sách Destination từ request (nếu có)
            if (request.Destinations != null && request.Destinations.Any())
            {
                foreach (var dest in request.Destinations)
                {
                    // Khởi tạo TourDestination với thông tin từ DTO
                    var newTourDestination = new TourDestination(
                        tour.Id,
                        dest.DestinationId,
                        dest.StartTime,
                        dest.EndTime,
                        dest.SortOrder,
                        dest.SortOrderByDate
                    );
                    _context.ImageUrls.RemoveRange(_context.ImageUrls.Where(i => i.RefId == newTourDestination.Id));
                    _context.ImageUrls.Add(new ImageUrl(newTourDestination.Id, dest.Img));
                    tour.TourDestinations.Add(newTourDestination);
                }
            }
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO response
            return ApiResponse<string>.SuccessResult("Tour destinations updated successfully", "Update successful");
        }
    }
}