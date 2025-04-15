using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using Domain.DataModel;
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
    
  

    // Command cập nhật danh sách TourDestination cho một Tour (Merge update)
    public record UpdateTourDestinationCommand(
        Guid TourId,
        List<DestinationToAdd> Destinations
    ) : IRequest<ApiResponse<string>>;

    public class UpdateTourDestinationHandler : IRequestHandler<UpdateTourDestinationCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTourDestinationHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(UpdateTourDestinationCommand request, CancellationToken cancellationToken)
        {
            // Lấy Tour theo TourId, bao gồm TourDestinations và các DestinationActivities liên quan
            var tour = await _context.Tours
                .Include(t => t.TourDestinations)
                    .ThenInclude(td => td.DestinationActivities)
                .Include(t => t.TourDestinations)
                    .ThenInclude(td => td.Destination)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

            if (tour == null)
            {
                return ApiResponse<string>.Failure("Tour not found", 404);
            }
            // Cast _context sang DbContext để sử dụng Entry cho các thuộc tính private
            var dbContext = _context as DbContext;
            if (dbContext == null)
            {
                throw new Exception("The IDtpDbContext instance is not a DbContext. Ensure your context implements Microsoft.EntityFrameworkCore.DbContext.");
            }
            // Lấy danh sách Destination mới từ request
            var newDestinations = request.Destinations;
            // Lấy danh sách Destination hiện tại theo DestinationId
            var existingDestinations = tour.TourDestinations.ToList();

            // Cập nhật hoặc thêm mới từng Destination
            foreach (var newDest in newDestinations)
            {
                // Giả sử rằng mỗi TourDestination được định danh duy nhất bằng DestinationId (cho một Tour)
                var existing = existingDestinations.FirstOrDefault(ed => ed.DestinationId == newDest.DestinationId);
                if (existing != null)
                {
                    // Cập nhật các trường nếu thay đổi (sử dụng _context.Entry để cập nhật các thuộc tính private)
                    dbContext.Entry(existing).Property("StartTime").CurrentValue = newDest.StartTime ?? TimeSpan.Zero;
                    dbContext.Entry(existing).Property("EndTime").CurrentValue = newDest.EndTime ?? TimeSpan.Zero;
                    dbContext.Entry(existing).Property("SortOrder").CurrentValue = newDest.SortOrder;
                    dbContext.Entry(existing).Property("SortOrderByDate").CurrentValue = newDest.SortOrderByDate;

                    // Cập nhật ImageUrls: Xóa những ảnh cũ và thêm ảnh mới nếu cần
                    var existingImages = _context.ImageUrls.Where(i => i.RefId == existing.Id).ToList();
                    _context.ImageUrls.RemoveRange(existingImages);
                    foreach (var img in newDest.Img)
                    {
                        _context.ImageUrls.Add(new ImageUrl(existing.Id, img));
                    }

                    // Xử lý DestinationActivities
                    var newActivities = newDest.DestinationActivities ?? new List<DestinationActivityToAdd>();
                    var existingActivities = existing.DestinationActivities.ToList();

                    // Cập nhật hoặc thêm mới các Activity
                    foreach (var newAct in newActivities)
                    {
                        // Cố gắng tìm activity tồn tại bằng cách so sánh Name (có thể cần so sánh thêm thời gian nếu cần)
                        var matchedActivity = existingActivities.FirstOrDefault(ea => ea.Name == newAct.Name);
                        if (matchedActivity != null)
                        {
                            dbContext.Entry(matchedActivity).Property("StartTime").CurrentValue = newAct.StartTime ?? TimeSpan.Zero;
                            dbContext.Entry(matchedActivity).Property("EndTime").CurrentValue = newAct.EndTime ?? TimeSpan.Zero;
                            dbContext.Entry(matchedActivity).Property("SortOrder").CurrentValue = newAct.SortOrder;
                        }
                        else
                        {
                            // Thêm mới activity
                            var activity = new DestinationActivity(
                                existing.Id,
                                newAct.Name,
                                newAct.StartTime ?? TimeSpan.Zero,
                                newAct.EndTime ?? TimeSpan.Zero,
                                newAct.SortOrder
                            );
                            existing.DestinationActivities.Add(activity);
                        }
                    }
                    // Xóa các activity hiện có mà không có trong newActivities
                    foreach (var existAct in existingActivities)
                    {
                        if (!newActivities.Any(na => na.Name == existAct.Name))
                        {
                            existing.DestinationActivities.Remove(existAct);
                        }
                    }
                }
                else
                {
                    // Nếu destination không tồn tại, tạo mới
                    var tourDestination = new TourDestination(
                        tour.Id,
                        newDest.DestinationId,
                        newDest.StartTime ?? TimeSpan.Zero,
                        newDest.EndTime ?? TimeSpan.Zero,
                        newDest.SortOrder,
                        newDest.SortOrderByDate
                    );
                    // Thêm image cho TourDestination
                    foreach (var img in newDest.Img)
                    {
                        _context.ImageUrls.Add(new ImageUrl(tourDestination.Id, img));
                    }
                    // Thêm các DestinationActivity nếu có
                    if (newDest.DestinationActivities != null)
                    {
                        foreach (var act in newDest.DestinationActivities)
                        {
                            var destinationActivity = new DestinationActivity(
                                tourDestination.Id,
                                act.Name,
                                act.StartTime ?? TimeSpan.Zero,
                                act.EndTime ?? TimeSpan.Zero,
                                act.SortOrder
                            );
                            tourDestination.DestinationActivities.Add(destinationActivity);
                        }
                    }
                    tour.TourDestinations.Add(tourDestination);
                }
            }

            // Xóa các TourDestination hiện có nhưng không có trong danh sách mới
            foreach (var existDest in existingDestinations)
            {
                if (!newDestinations.Any(nd => nd.DestinationId == existDest.DestinationId))
                {
                    tour.TourDestinations.Remove(existDest);
                }
            }

            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour destinations updated successfully", "Update successful");
        }
    }
}
