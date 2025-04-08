using Application.Common;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // Command để đóng tour; client chỉ cần truyền TourId
    public record CloseTourCommand(Guid TourId) : IRequest<ApiResponse<string>>;

    public class CloseTourHandler : IRequestHandler<CloseTourCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public CloseTourHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(CloseTourCommand request, CancellationToken cancellationToken)
        {
            // Tìm Tour theo TourId
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
            if (tour == null)
            {
                return ApiResponse<string>.Failure("Tour not found", 404);
            }
            if (!tour.IsDeleted)
            {
                return ApiResponse<string>.SuccessResult("Tour is already closed", "No update needed");
            }

            // Đóng tour: cập nhật IsDeleted thành false
            tour.IsDeleted = true;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour closed successfully", "Closed");
        }
    }
}
