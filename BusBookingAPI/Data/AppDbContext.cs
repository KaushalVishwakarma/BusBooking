using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<Models.Route> Routes => Set<Models.Route>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Bus>()
            .HasIndex(b => b.BusNumber).IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.BusId, s.SeatNumber }).IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.PNRNumber).IsUnique();

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.TransactionId).IsUnique();

        modelBuilder.Entity<BookingSeat>()
            .HasIndex(bs => new { bs.BookingId, bs.SeatId }).IsUnique();

        // Decimal precision
        modelBuilder.Entity<Schedule>()
            .Property(s => s.PricePerSeat).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalAmount).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Models.Route>()
            .Property(r => r.DistanceKm).HasColumnType("decimal(8,2)");
    }
}
