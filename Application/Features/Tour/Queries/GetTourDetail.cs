using Application.Contracts.Persistence;
using Domain.Entities;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries;

public record TourDestinationResponse
{
    public string Name { get; set; } = null!;

    public List<string> ImageUrls { get; set; } = new();

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public int? SortOrder { get; set; }
}

public record TourTemplateDetailsResponse
{
    public Guid Id { get; init; }

    public string Title { get; set; } = null!;

    public string CompanyName { get; set; }

    public string? Description { get; set; }

    public double AvgStar { get; set; }

    public int TotalRating { get; set; }

    public decimal OnlyFromCost { get; set; }

    public List<TicketType> TicketTypes { get; set; } = new();
}

public record TourDetailResponse
{
    public TourTemplateDetailsResponse Tour { get; init; }
    public List<Rating> Ratings { get; init; } = new();
    public List<TourDestinationResponse> TourDestinations { get; init; } = new();
};

public record GetTourDetail(Guid TourId) : IRequest<Option<TourDetailResponse>>;

public class GetTourDetailHandler(IDtpDbContext context) : IRequestHandler<GetTourDetail, Option<TourDetailResponse>>
{
    public Task<Option<TourDetailResponse>> Handle(GetTourDetail request, CancellationToken cancellationToken)
    {
        var tourDetailResponse = context.Tours
            .Include(t => t.Ratings)
            .Include(t => t.Company)
            .Include(t => t.Tickets)
            .Include(t => t.TourDestinations)
            .ThenInclude(td => td.Destination)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(t => t.Id == request.TourId)
            .Select(t => new TourDetailResponse
            {
                Tour = new TourTemplateDetailsResponse()
                {
                    Id = t.Id,
                    Title = t.Title,
                    CompanyName = t.Company.Name,
                    Description = t.Description,
                    AvgStar = t.Ratings.Any() ? t.Ratings.Average(rating => rating.Star) : 0,
                    TotalRating = t.Ratings.Count(),
                    OnlyFromCost = t.OnlyFromCost(),
                    TicketTypes = t.Tickets
                },
                Ratings = t.Ratings.ToList(),
                TourDestinations = t.TourDestinations
                    .Select(td => new TourDestinationResponse
                    {
                        Name = td.Destination.Name,
                        SortOrder = td.SortOrder,
                        StartTime = td.StartTime,
                        EndTime = td.EndTime,
                        ImageUrls = context.ImageUrls
                            .Where(img => img.RefId == td.Id)
                            .Select(img => img.Url)
                            .ToList()
                    })
                    .ToList()
            })
            .SingleOrDefault();

        return Task.FromResult(tourDetailResponse is null
            ? Option.None
            : Option.Some(tourDetailResponse));
    }
}