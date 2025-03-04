using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands;

public record DestinationToAdd(
    Guid DestinationId,
    DateTime StartTime,
    DateTime EndTime,
    int? SortOrder = null
);

public record AddDestinationsToTourCommand(
    Guid TourId,
    List<DestinationToAdd> Destinations
) : IRequest<ApiResponse<TourResponse>>;

public class AddDestinationsToTourHandler : IRequestHandler<AddDestinationsToTourCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public AddDestinationsToTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(AddDestinationsToTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours
                    .Include(t => t.TourDestinations)
                    .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
        if (tour == null)
        {
            return ApiResponse<TourResponse>.Failure("Tour not found", 404);
        }

        foreach (var dest in request.Destinations)
        {
            var existing = tour.TourDestinations.FirstOrDefault(td => td.DestinationId == dest.DestinationId);
            if (existing == null)
            {
                var tourDestination = new TourDestination(tour.Id, dest.DestinationId, dest.StartTime, dest.EndTime, dest.SortOrder);
                tour.TourDestinations.Add(tourDestination);
            }
        }

        _context.Tours.Update(tour);
        await _context.SaveChangesAsync(cancellationToken);

        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Destinations added successfully");
    }
}
