using Application.Contracts.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DtpDbContext(DbContextOptions<DtpDbContext> options) : IdentityDbContext<User>(options), IDtpDbContext
{
    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourBooking> TourBookings { get; set; }

    public virtual DbSet<TourDestination> TourDestinations { get; set; }

    public virtual DbSet<TourSchedule> TourSchedules { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Basket> Baskets { get; set; }
    
    public virtual DbSet<TourBasketItem> TourBasketItems { get; set; }
    
    public virtual DbSet<Ticket> Tickets { get; set; }
    
    public virtual DbSet<TicketType> TicketTypes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DtpDbContext).Assembly);
    }
}