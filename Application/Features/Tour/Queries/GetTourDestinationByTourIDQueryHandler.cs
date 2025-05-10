using Application.Contracts.Persistence;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Queries
{
    public record DestinationActivity(
        string Name,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int? SortOrder
    );
    public record TourDestinationDto(
        Guid Id,
        Guid DestinationId,
        List<DestinationActivity>? DestinationActivities,
        string DestinationName,
        TimeSpan? StartTime,
        TimeSpan? EndTime,
        int? SortOrder,
        int? SortOrderByDate,
        List<string> img 
    );

    // Query: lấy danh sách TourDestination theo TourId
    public record GetTourDestinationByTourIDQuery(Guid TourId) : IRequest<ApiResponse<List<TourDestinationDto>>>;

    public class GetTourDestinationByTourIDQueryHandler : IRequestHandler<GetTourDestinationByTourIDQuery, ApiResponse<List<TourDestinationDto>>>
    {
        private readonly IDtpDbContext _context;

        public GetTourDestinationByTourIDQueryHandler(IDtpDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<TourDestinationDto>>> Handle(GetTourDestinationByTourIDQuery request, CancellationToken cancellationToken)
        {
            // Lấy các TourDestination bao gồm Destination và DestinationActivities
            var tourDestinations = await _context.TourDestinations
                .Include(td => td.Destination)
                .Include(td => td.DestinationActivities)
                .Where(td => td.TourId == request.TourId)
                .ToListAsync(cancellationToken);

            // Tải tất cả ImageUrls vào bộ nhớ
            var imageUrls = await _context.ImageUrls.ToListAsync(cancellationToken);

            var dtoList = tourDestinations.Select(td => new TourDestinationDto(
                Id: td.Id,
                DestinationId: td.DestinationId,
                DestinationActivities: td.DestinationActivities?.Select(da => new DestinationActivity(
                    da.Name,
                    da.StartTime,
                    da.EndTime,
                    da.SortOrder
                )).ToList() ?? new List<DestinationActivity>(),
                DestinationName: td.Destination.Name,
                StartTime: td.StartTime,
                EndTime: td.EndTime,
                SortOrder: td.SortOrder,
                SortOrderByDate: td.SortOrderByDate,
                img: imageUrls.Where(i => i.RefId == td.Id).Select(i => i.Url).ToList()
            )).ToList();

            return ApiResponse<List<TourDestinationDto>>.SuccessResult(dtoList, "Tour destinations retrieved successfully");
        }

    }
}
