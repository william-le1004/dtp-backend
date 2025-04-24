using Application.Contracts.Persistence;
using Application.Extensions;
using Domain.Entities;
using Functional.Option;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries;

public record GetTourDetail(Guid TourId) : IRequest<Option<TourDetailResponse>>;

public class GetTourDetailHandler(IDtpDbContext context) : IRequestHandler<GetTourDetail, Option<TourDetailResponse>>
{
    public Task<Option<TourDetailResponse>> Handle(GetTourDetail request, CancellationToken cancellationToken)
    {
        var tourDetailResponse = (from t in context.Tours.IsDeleted(false)
            where t.Id == request.TourId
            select new TourDetailResponse
            {
                Tour = new TourTemplateDetailsResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    CompanyName = t.Company.Name,
                    Description = t.Description,
                    About = t.About ?? string.Empty,
                    AvgStar = t.Ratings.Any() ? t.Ratings.Average(rating => rating.Star) : 0,
                    TotalRating = t.Ratings.Count(),
                    OnlyFromCost = t.OnlyFromCost(),
                    TicketTypes = t.Tickets,
                    Pickinfor = t.Pickinfor,
                    Include = t.Include,
                    ImageUrls = (from img in context.ImageUrls
                        where img.RefId == t.Id
                        select img.Url).ToList()
                },
                TourDestinations = (from td in t.TourDestinations
                    select new TourDestinationResponse
                    {
                        Name = td.Destination.Name,
                        Longitude = td.Destination.Longitude,
                        Latitude = td.Destination.Latitude,
                        SortOrder = td.SortOrder,
                        SortOrderByDate = td.SortOrderByDate,
                        StartTime = td.StartTime,
                        EndTime = td.EndTime,
                        ImageUrls = (from img in context.ImageUrls
                            where img.RefId == td.Id
                            select img.Url).ToList(),
                        Activities = (from x in td.DestinationActivities
                            select new TourActivity
                            {
                                Name = x.Name,
                                StartTime = x.StartTime,
                                EndTime = x.EndTime,
                                SortOrder = x.SortOrder ?? 0
                            }).ToList()
                    }).ToList()
            }).SingleOrDefault();

        return Task.FromResult(tourDetailResponse is null
            ? Option.None
            : Option.Some(tourDetailResponse));
    }
}

public record TourDetailResponse
{
    public TourTemplateDetailsResponse Tour { get; init; }
    public List<TourDestinationResponse> TourDestinations { get; init; } = new();
}

public record TourTemplateDetailsResponse
{
    public Guid Id { get; init; }

    public string Title { get; set; } = null!;

    public string CompanyName { get; set; }

    public string? Description { get; set; }

    public double AvgStar { get; set; }

    public int TotalRating { get; set; }

    public string About { get; set; }

    public decimal OnlyFromCost { get; set; }
    
    public string? Include { get; set; }
    
    public string? Pickinfor { get; set; }

    public List<TicketType> TicketTypes { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}

public record TourDestinationResponse
{
    public string Name { get; set; } = null!;

    public List<string> ImageUrls { get; set; } = new();

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public int? SortOrder { get; set; }
    
    public int? SortOrderByDate { get; set; }
    
    public string Latitude { get; set; } = null!;

    public string Longitude { get; set; } = null!;

    public List<TourActivity> Activities { get; set; } = new();
}

public record TourActivity
{
    public string Name { get; set; } = null!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SortOrder { get; set; }
}