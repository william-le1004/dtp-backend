using Application.Contracts.Persistence;
using Application.Common;
using Application.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands;

public record PutTourCommand(Guid TourId, string Title, Guid? CompanyId, Guid? Category, string? Description)
    : IRequest<ApiResponse<TourResponse>>;

public class PutTourHandler : IRequestHandler<PutTourCommand, ApiResponse<TourResponse>>
{
    private readonly IDtpDbContext _context;

    public PutTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TourResponse>> Handle(PutTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);
        if (tour is null)
        {
            return ApiResponse<TourResponse>.Failure("Tour not found", 404);
        }

        tour.Update(request.Title, request.CompanyId, request.Category, request.Description);

        _context.Tours.Update(tour);
        await _context.SaveChangesAsync(cancellationToken);

        var tourResponse = new TourResponse(tour.Id, tour.Title, tour.CompanyId, tour.Category, tour.Description);
        return ApiResponse<TourResponse>.SuccessResult(tourResponse, "Tour updated successfully");
    }
}
