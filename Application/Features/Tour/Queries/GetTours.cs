using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tour.Queries;

public record TourTemplateResponse
{
    public Guid Id { get; init; }

    public string ThumbnailUrl { get; set; }
    public string Title { get; set; } = null!;

    public string CompanyName { get; set; }

    public string? Description { get; set; }

    public double AvgStar { get; set; }

    public int TotalRating { get; set; }
    
    public decimal OnlyFromCost { get; set; }
}

public record GetTours() : IRequest<IEnumerable<TourTemplateResponse>>;

public class GetToursHandler(IDtpDbContext context) : IRequestHandler<GetTours, IEnumerable<TourTemplateResponse>>
{
    public async Task<IEnumerable<TourTemplateResponse>> Handle(GetTours request, CancellationToken cancellationToken)
    {
        var tours = context.Tours.Include(tour => tour.Company)
            .Include(tour => tour.Ratings)
            .Include(tour => tour.Tickets)
            .Select(tour => new TourTemplateResponse()
            {
                Id = tour.Id,
                ThumbnailUrl = context.ImageUrls.Any(image => image.RefId == tour.Id)
                    ? context.ImageUrls.FirstOrDefault(image => image.RefId == tour.Id).Url : null,
                Title = tour.Title,
                CompanyName = tour.Company.Name,
                Description = tour.Description,
                AvgStar = tour.Ratings.Any() ? tour.Ratings.Average(rating => rating.Star) : 0,
                TotalRating = tour.Ratings.Count(),
                OnlyFromCost = tour.OnlyFromCost()
            });

        return await tours.ToListAsync(cancellationToken);
    }
}

