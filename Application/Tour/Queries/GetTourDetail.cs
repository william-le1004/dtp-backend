using Domain.Entities;
using Functional.Option;
using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tour.Queries;

public record TourDestinationResponse
{
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public int? SortOrder { get; set; }
}

public record TourDetailResponse
{
    public TourResponse Tour { get; init; }
    public List<Rating> Ratings { get; init; } = new();
    public List<TourDestinationResponse> TourDestinations { get; init; } = new();
};

public record GetTourDetail(Guid TourId) : IRequest<Option<TourDetailResponse>>;

public class GetTourDetailHandler(DtpDbContext context) : IRequestHandler<GetTourDetail, Option<TourDetailResponse>>
{
    public Task<Option<TourDetailResponse>> Handle(GetTourDetail request, CancellationToken cancellationToken)
    {
        var tourDetailResponse = context.Tours
            .Include(t => t.Ratings)
            .Include(t => t.Company)
            .Include(t => t.TourDestinations)
            .ThenInclude(td => td.Destination)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(t => t.Id == request.TourId)
            .Select(t => new TourDetailResponse
            {
                Tour = new TourResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    CompanyName = t.Company.Name,
                    Description = t.Description,
                    AvgStar = t.Ratings.Any() ? t.Ratings.Average(rating => rating.Star) : 0,
                    TotalRating = t.Ratings.Count(),
                },
                Ratings = t.Ratings.ToList(),  // Assuming Ratings is a collection you want to include
                TourDestinations = t.TourDestinations
                    .Select(td => new TourDestinationResponse
                    {
                        Name = td.Destination.Name,
                        Image = td.Destination.Image,
                        SortOrder = td.SortOrder
                    })
                    .ToList()
            })
            .SingleOrDefault();

        return Task.FromResult(tourDetailResponse is null
            ? Option.None
            : Option.Some(tourDetailResponse));
    }
}