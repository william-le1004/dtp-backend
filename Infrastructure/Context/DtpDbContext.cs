using System;
using System.Collections.Generic;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<DtpDbContext>
{
    public DtpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DtpDbContext>();
        optionsBuilder.UseMySQL("Server=MYSQL1001.site4now.net;Database=db_ab3495_dtp;Uid=ab3495_dtp;Pwd=dtpct123");

        return new DtpDbContext(optionsBuilder.Options);
    }
}

public class DtpDbContext(DbContextOptions<DtpDbContext> options) : DbContext(options)
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

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }
    public virtual DbSet<Basket> Baskets { get; set; }
}