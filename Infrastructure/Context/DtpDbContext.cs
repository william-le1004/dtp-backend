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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configurations(modelBuilder);
    }

    private static void Configurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Company");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.License, "License").IsUnique();

            entity.HasIndex(e => e.Phone, "Phone").IsUnique();

            entity.HasIndex(e => e.TaxCode, "TaxCode").IsUnique();


            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.License).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Feedback");

            entity.HasIndex(e => e.TourId, "TourID");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasDefaultValueSql("'System'");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModified)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(255)
                .HasDefaultValueSql("'System'");
            entity.Property(e => e.TourId).HasColumnName("TourID");

            entity.HasOne(d => d.Tour).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("Feedback_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Feedback_ibfk_2");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BookingId, "BookingID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.PaymentMethod).HasColumnType("enum('credit_card','paypal','bank_transfer')");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Rating");

            entity.HasIndex(e => e.TourId, "TourID");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasDefaultValueSql("'System'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModified)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(255)
                .HasDefaultValueSql("'System'");
            entity.Property(e => e.Star).HasColumnName("Star");
            entity.Property(e => e.TourId).HasColumnName("TourID");

            entity.HasOne(d => d.Tour).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("Rating_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Rating_ibfk_2");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.CompanyId, "CompanyId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Company).WithMany(p => p.Tours)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Tours_ibfk_1");
        });

        modelBuilder.Entity<TourBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.TourScheduleId, "TourScheduleID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasPrecision(10);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Remark).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','canceled')");
            entity.Property(e => e.TourScheduleId).HasColumnName("TourScheduleID");

            entity.HasOne(d => d.TourSchedule).WithMany(p => p.TourBookings)
                .HasForeignKey(d => d.TourScheduleId)
                .HasConstraintName("TourBookings_ibfk_1");
        });

        modelBuilder.Entity<TourDestination>(entity =>
        {
            entity.HasKey(e => new { e.TourId, e.DestinationId }).HasName("PRIMARY");

            entity.HasIndex(e => e.DestinationId, "DestinationID");

            entity.Property(e => e.TourId).HasColumnName("TourID");
            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.SortOrder).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Destination).WithMany(p => p.TourDestinations)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("TourDestinations_ibfk_2");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourDestinations)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("TourDestinations_ibfk_1");
        });

        modelBuilder.Entity<TourSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.TourId, "TourID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Remark).HasColumnType("text");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'upcoming'")
                .HasColumnType("enum('upcoming','ongoing','completed','canceled')");
            entity.Property(e => e.TourId).HasColumnName("TourID");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourSchedules)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("TourSchedules_ibfk_1");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.WalletId, "WalletID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasPrecision(10);
            entity.Property(e => e.BankAccountId).HasColumnName("BankAccountID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','completed','failed')");
            entity.Property(e => e.WalletId).HasColumnName("WalletID");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("Transactions_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Balance)
                .HasPrecision(15)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Wallet");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance).HasPrecision(15);

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Wallet_ibfk_1");
        });
    }
}
