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
        TimeSpan StartTime,
        TimeSpan EndTime,
        int? SortOrder,
        int? SortOrderByDate,
        string? Img 
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
            // Truy vấn TourDestinations bao gồm Destination và DestinationActivities
            var tourDestinations = await _context.TourDestinations
                .Include(td => td.Destination)
                .Include(td => td.DestinationActivities)
                .Where(td => td.TourId == request.TourId)
                .ToListAsync(cancellationToken);

            // Chuyển đổi sang DTO
            var dtoList = tourDestinations.Select(td => new TourDestinationDto(
                Id: td.Id,
                DestinationId: td.DestinationId,
                DestinationActivities: td.DestinationActivities != null
                    ? td.DestinationActivities.Select(da => new DestinationActivity(
                        da.Name,
                        da.StartTime,
                        da.EndTime,
                        da.SortOrder
                    )).ToList()
                    : new List<DestinationActivity>(),
                DestinationName: td.Destination.Name,
                StartTime: td.StartTime,
                EndTime: td.EndTime,
                SortOrder: td.SortOrder,
                SortOrderByDate: td.SortOrderByDate,
                Img: _context.ImageUrls.Any(i => i.RefId == td.Id)
                        ? _context.ImageUrls.FirstOrDefault(i => i.RefId == td.Id)?.Url
                        : null
            )).ToList();

            return ApiResponse<List<TourDestinationDto>>.SuccessResult(dtoList, "Tour destinations retrieved successfully");
        }
    }
}
