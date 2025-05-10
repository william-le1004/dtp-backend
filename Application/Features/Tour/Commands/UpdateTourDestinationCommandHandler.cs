using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public class UpdateTourDestinationHandler
        : IRequestHandler<UpdateTourDestinationCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;

        public UpdateTourDestinationHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(
            UpdateTourDestinationCommand request,
            CancellationToken ct)
        {
            // 1) Load toàn bộ Destinations cũ
            var oldTds = await _context.TourDestinations
                .Where(td => td.TourId == request.TourId)
                .Include(td => td.DestinationActivities)
                .ToListAsync(ct);

            if (oldTds == null)
                return ApiResponse<string>.Failure("Tour không tồn tại", 404);

            // 2) Xoá sạch ImageUrl liên quan
            var oldIds = oldTds.Select(td => td.Id).ToList();
            var oldImgs = await _context.ImageUrls
                .Where(i => oldIds.Contains(i.RefId))
                .ToListAsync(ct);
            _context.ImageUrls.RemoveRange(oldImgs);

            // 3) Xoá tất cả DestinationActivity cũ
            var oldActs = oldTds.SelectMany(td => td.DestinationActivities).ToList();
            _context.DestinationActivities.RemoveRange(oldActs);

            // 4) Xoá tất cả TourDestination cũ
            _context.TourDestinations.RemoveRange(oldTds);

            // 5) Commit xoá
            await _context.SaveChangesAsync(ct);

            // 6) Chèn lại toàn bộ theo danh sách mới
            foreach (var nd in request.Destinations)
            {
                var td = new TourDestination(
                    request.TourId,
                    nd.DestinationId,
                    nd.StartTime ?? TimeSpan.Zero,
                    nd.EndTime ?? TimeSpan.Zero,
                    nd.SortOrder,
                    nd.SortOrderByDate);

                // 6a) Ảnh
                if (nd.Img != null)
                    foreach (var url in nd.Img)
                        _context.ImageUrls.Add(new ImageUrl(td.Id, url));

                // 6b) Activities
                if (nd.DestinationActivities != null)
                {
                    foreach (var na in nd.DestinationActivities)
                    {
                        td.DestinationActivities.Add(new DestinationActivity(
                            td.Id,
                            na.Name!,
                            na.StartTime ?? TimeSpan.Zero,
                            na.EndTime ?? TimeSpan.Zero,
                            na.SortOrder));
                    }
                }

                _context.TourDestinations.Add(td);
            }

            // 7) Commit chèn
            await _context.SaveChangesAsync(ct);

            return ApiResponse<string>.SuccessResult(
                "Cập nhật TourDestinations thành công",
                "Update successful");
        }
    }
}
