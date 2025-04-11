using Application.Contracts.Persistence;
using Application.Dtos;
using Application.Extensions;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries;

public record TourTemplateResponse : AuditResponse
{
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; } = null!;

    public string CompanyName { get; set; }

    public string? Description { get; set; }

    public double AvgStar { get; set; }

    public int TotalRating { get; set; }

    public decimal OnlyFromCost { get; set; }

    public IEnumerable<TourScheduleResponse> TourScheduleResponses { get; set; }
}

public record TourScheduleResponse
{
    public Guid Id { get; set; }
    public DateTime OpenDate { get; set; }
}

public record GetTours() : IRequest<IQueryable<TourTemplateResponse>>;

public class GetToursHandler(IDtpDbContext context) : IRequestHandler<GetTours, IQueryable<TourTemplateResponse>>
{
    public Task<IQueryable<TourTemplateResponse>> Handle(GetTours request, CancellationToken cancellationToken)
    {
        var tours = context.Tours.IsDeleted(false)
            .Include(tour => tour.Company)
            .Include(tour => tour.TourSchedules)
            .Include(tour => tour.Ratings)
            .Include(tour => tour.Tickets)
            .AsSingleQuery()
            .AsNoTracking()
            .Select(tour => new TourTemplateResponse()
            {
                Id = tour.Id,
                ThumbnailUrl = context.ImageUrls.Any(image => image.RefId == tour.Id)
                    ? context.ImageUrls.FirstOrDefault(image => image.RefId == tour.Id).Url
                    : null,
                Title = tour.Title,
                CompanyName = tour.Company.Name,
                Description = tour.Description,
                AvgStar = tour.Ratings.Any() ? tour.Ratings.Average(rating => rating.Star) : 0,
                TotalRating = tour.Ratings.Count(),
                OnlyFromCost = tour.Tickets.Min(x => x.DefaultNetCost),
                IsDeleted = tour.IsDeleted,
                CreatedAt = tour.CreatedAt,
                TourScheduleResponses =
    tour.TourSchedules.Select(schedule => new TourScheduleResponse
    {
        Id = schedule.Id,
        OpenDate = schedule.OpenDate.HasValue ? schedule.OpenDate.Value : DateTime.MinValue
    })

            });

        return Task.FromResult(tours.AsQueryable());
    }
}