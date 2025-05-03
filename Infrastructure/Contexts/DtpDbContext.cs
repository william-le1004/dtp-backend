using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.DataModel;
using Domain.Entities;
using Domain.ValueObject;
using Infrastructure.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DtpDbContext(DbContextOptions<DtpDbContext> options, IMediator mediator, IUserContextService userContext, AuditableEntityInterceptor auditInterceptor)
    : IdentityDbContext<User>(options), IDtpDbContext
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
    public virtual DbSet<TourScheduleTicket> TourScheduleTicket { get; set; }
    public virtual DbSet<ImageUrl> ImageUrls { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<DestinationActivity> DestinationActivities { get; set; }
    public virtual DbSet<Voucher> Voucher { get; set; }
    public virtual DbSet<ExternalTransaction> ExternalTransaction { get; set; }
    public virtual DbSet<SystemSetting> SystemSetting { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DtpDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await mediator.DispatchDomainEventsAsync(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}