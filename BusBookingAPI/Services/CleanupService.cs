using BusBookingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class CleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupService> _logger;

    public CleanupService(IServiceScopeFactory scopeFactory, ILogger<CleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run once immediately on startup, then every 24 hours
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCleanup();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunCleanup()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var today = DateTime.Today;

        // 1. Mark past schedules as Completed
        var pastSchedules = await db.Schedules
            .Where(s => s.DepartureTime.Date < today && s.Status == "Scheduled")
            .ToListAsync();

        foreach (var s in pastSchedules)
            s.Status = "Completed";

        await db.SaveChangesAsync();

        // 2. Find buses with no future scheduled trips
        var busesWithFutureSchedules = await db.Schedules
            .Where(s => s.DepartureTime.Date >= today && s.Status == "Scheduled")
            .Select(s => s.BusId)
            .Distinct()
            .ToListAsync();

        var busIdsToRemove = await db.Buses
            .Where(b => !busesWithFutureSchedules.Contains(b.BusId))
            .Select(b => b.BusId)
            .ToListAsync();

        if (!busIdsToRemove.Any())
        {
            _logger.LogInformation("Cleanup: {schedules} schedules completed, 0 buses removed.", pastSchedules.Count);
            return;
        }

        // 3. Delete in correct FK order: BookingSeats → Bookings → Payments → Schedules → Seats → Buses
        var scheduleIds = await db.Schedules
            .Where(s => busIdsToRemove.Contains(s.BusId))
            .Select(s => s.ScheduleId)
            .ToListAsync();

        var bookingIds = await db.Bookings
            .Where(b => scheduleIds.Contains(b.ScheduleId))
            .Select(b => b.BookingId)
            .ToListAsync();

        // BookingSeats
        var bookingSeats = await db.BookingSeats
            .Where(bs => bookingIds.Contains(bs.BookingId))
            .ToListAsync();
        db.BookingSeats.RemoveRange(bookingSeats);

        // Payments
        var payments = await db.Payments
            .Where(p => bookingIds.Contains(p.BookingId))
            .ToListAsync();
        db.Payments.RemoveRange(payments);

        // Bookings
        var bookings = await db.Bookings
            .Where(b => bookingIds.Contains(b.BookingId))
            .ToListAsync();
        db.Bookings.RemoveRange(bookings);

        // Schedules
        var schedules = await db.Schedules
            .Where(s => busIdsToRemove.Contains(s.BusId))
            .ToListAsync();
        db.Schedules.RemoveRange(schedules);

        // Seats
        var seats = await db.Seats
            .Where(s => busIdsToRemove.Contains(s.BusId))
            .ToListAsync();
        db.Seats.RemoveRange(seats);

        // Buses
        var buses = await db.Buses
            .Where(b => busIdsToRemove.Contains(b.BusId))
            .ToListAsync();
        db.Buses.RemoveRange(buses);

        await db.SaveChangesAsync();

        _logger.LogInformation(
            "Cleanup: {schedules} schedules completed, {buses} buses removed.",
            pastSchedules.Count, busIdsToRemove.Count);
    }
}
