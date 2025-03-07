using Application.Contracts.Persistence;
using Domain.DataModel;
using Domain.Entities;
using Domain.Enum;
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

    public virtual DbSet<ImageUrl> ImageUrls { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    
    public virtual DbSet<TourScheduleTicket> TourScheduleTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DtpDbContext).Assembly);
        // SeedData(modelBuilder);
    }

    public void SeedData(ModelBuilder modelBuilder)
    {
        var destination1Id = Guid.NewGuid();
        var destination2Id = Guid.NewGuid();
        var company1Id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = categoryId,
                Name = "Tour trong ngày",
            },
            new Category
            {
                Id = categoryId2,
                Name = "Tour nhiều ngày",
            }
            
            );

        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = company1Id,
                Name = "Tech Innovators Inc.",
                Email = "contact@techinnovators.com",
                Phone = "123-456-7890",
                TaxCode = "ABC123456",
                License = "LICENSE1234",
                CreatedBy = "admin",
            }
        );

        modelBuilder.Entity<Destination>().HasData(
            new Destination
            {
                Id = destination1Id, Name = "Tropical Beach", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            },
            new Destination
            {
                Id = destination2Id, Name = "Mountain Peak", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            }
        );

        // Create Tours next
        var tour1Id = Guid.NewGuid();
        var tour2Id = Guid.NewGuid();

        modelBuilder.Entity<Tour>().HasData(
            new Tour
            {
                Id = tour1Id, Title = "Beach Tour", CompanyId = company1Id, CategoryId = categoryId,
                Description = "A beautiful beach tour", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            },
            new Tour
            {
                Id = tour2Id, Title = "Mountain Tour", CompanyId = company1Id, CategoryId = categoryId2,
                Description = "Explore the mountains", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            }
        );
        
        var ticket1Id = Guid.NewGuid();
        var ticket2Id = Guid.NewGuid();
        var ticket3Id = Guid.NewGuid();
        var ticket4Id = Guid.NewGuid();

        // Create TicketTypes and link to Tours
        modelBuilder.Entity<TicketType>().HasData(
            new TicketType
            {
                Id = ticket1Id, DefaultNetCost = 70.00m, MinimumPurchaseQuantity = 1,
                TicketKind = TicketKind.Adult, TourId = tour1Id
            },
            new TicketType
            {
                Id = ticket2Id, DefaultNetCost = 50.00m, MinimumPurchaseQuantity = 1,
                TicketKind = TicketKind.Child, TourId = tour1Id
            },
            new TicketType
            {
                Id = ticket3Id, DefaultNetCost = 150.00m, MinimumPurchaseQuantity = 1,
                TicketKind = TicketKind.Adult, TourId = tour2Id
            },
            new TicketType
            {
                Id = ticket4Id, DefaultNetCost = 75.00m, MinimumPurchaseQuantity = 2,
                TicketKind = TicketKind.Child, TourId = tour2Id
            }
        );
        // Create TourDestinations and link to Tours and Destinations
        modelBuilder.Entity<TourDestination>().HasData(
            new TourDestination
            {
                Id = Guid.NewGuid(), TourId = tour1Id, DestinationId = destination1Id,
                StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0), SortOrder = 1, SortOrderByDate = 1
            },
            new TourDestination
            {
                Id = Guid.NewGuid(), TourId = tour2Id, DestinationId = destination2Id,
                StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0), SortOrder = 2, SortOrderByDate = 2
            }
        );

        // Create TourSchedules and link to Tours
        var schedule1Id = Guid.NewGuid();
        var schedule2Id = Guid.NewGuid();

        modelBuilder.Entity<TourSchedule>().HasData(
            new TourSchedule
            {
                Id = schedule1Id, TourId = tour1Id, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5),
                PriceChangeRate = 1.2, Remark = "No special remarks", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            },
            new TourSchedule
            {
                Id = schedule2Id, TourId = tour2Id, StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(15), PriceChangeRate = 1.5, Remark = "Special offer",
                CreatedAt = DateTime.UtcNow, CreatedBy = "admin", IsDeleted = false
            }
        );

        // Create TourScheduleTickets and link to TourSchedules
        modelBuilder.Entity<TourScheduleTicket>().HasData(
            new TourScheduleTicket
            {
                TicketTypeId = ticket1Id, TourScheduleId = schedule1Id, NetCost = 100.00m, AvailableTicket = 20
            },
            new TourScheduleTicket
            {
                TicketTypeId = ticket2Id, TourScheduleId = schedule1Id, NetCost = 100.00m, AvailableTicket = 20
            },
            new TourScheduleTicket
            {
                TicketTypeId = ticket3Id, TourScheduleId = schedule2Id, NetCost = 100.00m, AvailableTicket = 20
            },
            new TourScheduleTicket
                { TicketTypeId = ticket4Id, TourScheduleId = schedule2Id, NetCost = 150.00m, AvailableTicket = 50 }
        );
    }
}