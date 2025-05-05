using Application.Common;
using Application.Contracts.Persistence;
using Domain.DataModel;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.Tour.Commands
{
    

    public record UpdateTourDestinationCommand(
        Guid TourId,
        List<DestinationToAdd> Destinations
    ) : IRequest<ApiResponse<string>>;

    public class UpdateTourDestinationHandler : IRequestHandler<UpdateTourDestinationCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;
        private readonly DbContext _dbContext;

        public UpdateTourDestinationHandler(IDtpDbContext context)
        {
            _context = context;
            _dbContext = context as DbContext ?? throw new ArgumentException();
        }

        public async Task<ApiResponse<string>> Handle(UpdateTourDestinationCommand request, CancellationToken ct)
        {
            var tour = await _context.Tours
                .Include(t => t.TourDestinations).ThenInclude(td => td.DestinationActivities)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, ct);
            if (tour == null) return ApiResponse<string>.Failure("Tour not found", 404);

            // Xóa cũ...
            var toRemove = tour.TourDestinations
                .Where(td => !request.Destinations.Any(nd => nd.DestinationId == td.DestinationId))
                .ToList();
            foreach (var oldTd in toRemove)
            {
                _context.ImageUrls.RemoveRange(
                    _context.ImageUrls.Where(i => i.RefId == oldTd.Id));
                _context.DestinationActivities.RemoveRange(oldTd.DestinationActivities);
                _context.TourDestinations.Remove(oldTd);
            }

            // Cập nhật/Thêm mới
            foreach (var nd in request.Destinations)
            {
                var existing = tour.TourDestinations
                                   .FirstOrDefault(td => td.DestinationId == nd.DestinationId);
                if (existing != null)
                {
                    // ... cập nhật StartTime, EndTime, SortOrder, SortOrderByDate
                    _dbContext.Entry(existing).Property(nameof(TourDestination.StartTime))
                              .CurrentValue = nd.StartTime ?? existing.StartTime;
                    _dbContext.Entry(existing).Property(nameof(TourDestination.EndTime))
                              .CurrentValue = nd.EndTime ?? existing.EndTime;
                    _dbContext.Entry(existing).Property(nameof(TourDestination.SortOrder))
                              .CurrentValue = nd.SortOrder;
                    _dbContext.Entry(existing).Property(nameof(TourDestination.SortOrderByDate))
                              .CurrentValue = nd.SortOrderByDate;

                    // Xóa ảnh cũ
                    _context.ImageUrls.RemoveRange(
                        _context.ImageUrls.Where(i => i.RefId == existing.Id));
                    // Thêm ảnh mới
                    if (nd.Img != null)
                    {
                        foreach (var url in nd.Img)
                            _context.ImageUrls.Add(new ImageUrl(existing.Id, url));
                    }

                    // Merge activities (tương tự)
                    // ...
                }
                else
                {
                    // Thêm mới TourDestination
                    var td = new TourDestination(
                        tour.Id,
                        nd.DestinationId,
                        nd.StartTime ?? TimeSpan.Zero,
                        nd.EndTime ?? TimeSpan.Zero,
                        nd.SortOrder,
                        nd.SortOrderByDate);
                    tour.TourDestinations.Add(td);

                    // Thêm ảnh
                    if (nd.Img != null)
                    {
                        foreach (var url in nd.Img)
                            _context.ImageUrls.Add(new ImageUrl(td.Id, url));
                    }
                    // Thêm activities...
                }
            }

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResponse<string>.Failure("Conflict", 409);
            }
            return ApiResponse<string>.SuccessResult("OK", "Updated");
        }
    }
}
