using Domain.DataModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Persistence;

public interface IDtpDbContext
{
    public DbSet<Company> Companies { get; set; }

    public DbSet<Destination> Destinations { get; set; }

    public DbSet<Feedback> Feedbacks { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    public DbSet<Domain.Entities.Tour> Tours { get; set; }

    public DbSet<TourBooking> TourBookings { get; set; }

    public DbSet<TourDestination> TourDestinations { get; set; }

    public DbSet<TourSchedule> TourSchedules { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

    public DbSet<Basket> Baskets { get; set; }

    public DbSet<TourBasketItem> TourBasketItems { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<TicketType> TicketTypes { get; set; }

    public DbSet<Category> Categories { get; set; }


    public DbSet<ImageUrl> ImageUrls { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}