using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos; // Giả sử TourResponse được định nghĩa: record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // DTO nhận thông tin Destination cần thêm vào Tour
    public record DestinationToUpdate(
        Guid DestinationId,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int? SortOrder = null,
        int? SortOrderByDate = null
    );

    // Command cập nhật danh sách TourDestination cho Tour:
    // Xóa toàn bộ các TourDestination hiện có và thêm danh sách mới.
    public record UpdateTourDestinationCommand(
        Guid TourId,
        List<DestinationToAdd> Destinations
    ) : IRequest<ApiResponse<TourResponse>>;

    public class UpdateTourDestinationHandler : IRequestHandler<UpdateTourDestinationCommand, ApiResponse<TourResponse>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTourDestinationHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<TourResponse>> Handle(UpdateTourDestinationCommand request, CancellationToken cancellationToken)
        {
            // Lấy Tour theo TourId kèm theo collection TourDestinations
            var tour = await _context.Tours
                .Include(t => t.TourDestinations)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
            if (tour == null)
            {
                return ApiResponse<TourResponse>.Failure("Tour not found", 404);
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
                    tour.TourDestinations.Add(newTourDestination);
                }
            }

            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            // Tạo DTO response
            var response = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
            return ApiResponse<TourResponse>.SuccessResult(response, "Tour destinations updated successfully");
        }
    }
}
