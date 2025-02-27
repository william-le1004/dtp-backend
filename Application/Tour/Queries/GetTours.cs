using Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tour.Queries;

public record TourResponse
{
    public Guid Id { get; init; }

    public string Title { get; set; } = null!;

    public string CompanyName { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public double AvgStar { get; set; }

    public int TotalRating { get; set; }
}

public record GetTours() : IRequest<IEnumerable<TourResponse>>;

public class GetToursHandler(DtpDbContext context) : IRequestHandler<GetTours, IEnumerable<TourResponse>>
{
    public async Task<IEnumerable<TourResponse>> Handle(GetTours request, CancellationToken cancellationToken)
    {
        var tours = context.Tours.Include(tour => tour.Company).Include(tour => tour.Ratings)
            .Select(tour => new TourResponse()
            {
                Id = tour.Id,
                Title = tour.Title,
                CompanyName = tour.Company.Name,
                Description = tour.Description,
                AvgStar = tour.Ratings.Any() ? tour.Ratings.Average(rating => rating.Star) : 0,
                TotalRating = tour.Ratings.Count(),
            });

        return await tours.ToListAsync(cancellationToken);
    }
}