using Application.Common;
using Application.Contracts.Persistence;
using Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries;

public record GetListTour() : IRequest<ApiResponse<List<TourResponse>>>;

public class GetListTourHandler : IRequestHandler<GetListTour, ApiResponse<List<TourResponse>>>
{
    private readonly IDtpDbContext _context;

    public GetListTourHandler(IDtpDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<TourResponse>>> Handle(GetListTour request, CancellationToken cancellationToken)
    {
        var tours = await _context.Tours.ToListAsync(cancellationToken);
        var tourDtos = tours.Select(t => new TourResponse(t.Id, t.Title, t.CompanyId, t.CategoryId, t.Description))
            .ToList();

        return ApiResponse<List<TourResponse>>.SuccessResult(tourDtos, "Tour list retrieved successfully");
    }
}