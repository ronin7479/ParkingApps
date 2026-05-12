using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ParkingApp.Models;
using System;
using System.IO;

namespace ParkingApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // DB file sits next to the .exe
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "parking.db");

            options.UseSqlite($"Data Source={dbPath}")
                   // Ігноруємо попередження про pending migrations
                   .ConfigureWarnings(w =>
                        w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Login).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Login).IsRequired().HasMaxLength(50);
                e.Property(u => u.Email).IsRequired().HasMaxLength(100);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            });

            // ParkingLots
            modelBuilder.Entity<ParkingLot>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                e.Property(p => p.Location).IsRequired().HasMaxLength(200);
                e.Property(p => p.PricePerHour).HasColumnType("decimal(10,2)");
            });

            // ParkingSpaces
            modelBuilder.Entity<ParkingSpace>(e =>
            {
                e.HasKey(s => s.Id);
                e.HasOne<ParkingLot>()
                 .WithMany()
                 .HasForeignKey(s => s.ParkingLotId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Bookings
            modelBuilder.Entity<Booking>(e =>
            {
                e.HasKey(b => b.Id);
                e.HasIndex(b => b.ReceiptNumber).IsUnique();
                e.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");
                e.Property(b => b.IsActive).HasDefaultValue(true);
                e.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne<ParkingLot>()
                 .WithMany()
                 .HasForeignKey(b => b.ParkingLotId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Receipts
            modelBuilder.Entity<Receipt>(e =>
            {
                e.HasKey(r => r.Id);
                e.HasIndex(r => r.ReceiptNumber).IsUnique();
                e.Property(r => r.Amount).HasColumnType("decimal(10,2)");
            });
        }
    }
}
