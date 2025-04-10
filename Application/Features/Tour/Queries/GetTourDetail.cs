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
        var tourDetailResponse = context.Tours.IsDeleted(false)
            .Include(t => t.Ratings)
            .Include(t => t.Company)
            .Include(t => t.Tickets)
            .Include(t => t.TourDestinations)
            .ThenInclude(td => td.Destination)
            .Include(t => t.TourDestinations)
            .ThenInclude(td => td.DestinationActivities)
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
                    About = t.About ?? string.Empty,
                    AvgStar = t.Ratings.Any() ? t.Ratings.Average(rating => rating.Star) : 0,
                    TotalRating = t.Ratings.Count(),
                    OnlyFromCost = t.OnlyFromCost(),
                    TicketTypes = t.Tickets
                },
                Ratings = t.Ratings.Select(r => new RatingResponse()
                {
                    Star = r.Star,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                }).OrderByDescending(x => x.CreatedAt).ToList(),
                TourDestinations = t.TourDestinations
                    .Select(td => new TourDestinationResponse
                    {
                        Name = td.Destination.Name,
                        SortOrder = td.SortOrder,
                        SortOrderByDate = td.SortOrderByDate,
                        StartTime = td.StartTime,
                        EndTime = td.EndTime,
                        ImageUrls = context.ImageUrls
                            .Where(img => img.RefId == td.Id)
                            .Select(img => img.Url)
                            .ToList(),
                        Activities = td.DestinationActivities.Select(x => new TourActivity()
                        {
                            Name = x.Name,
                            StartTime = x.StartTime,
                            EndTime = x.EndTime,
                            SortOrder = x.SortOrder ?? 0
                        }).ToList(),
                    })
                    .ToList()
            })
            .SingleOrDefault();

        return Task.FromResult(tourDetailResponse is null
            ? Option.None
            : Option.Some(tourDetailResponse));
    }
}

public record TourDetailResponse
{
    public TourTemplateDetailsResponse Tour { get; init; }
    public List<RatingResponse> Ratings { get; init; } = new();
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

    public List<TicketType> TicketTypes { get; set; } = new();
}

public record RatingResponse
{
    public int Star { get; set; }

    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record TourDestinationResponse
{
    public string Name { get; set; } = null!;

    public List<string> ImageUrls { get; set; } = new();

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public int? SortOrder { get; set; }
    
    public int? SortOrderByDate { get; set; }

    public List<TourActivity> Activities { get; set; } = new();
}

public record TourActivity
{
    public string Name { get; set; } = null!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SortOrder { get; set; }
}