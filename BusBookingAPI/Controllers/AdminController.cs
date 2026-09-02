using BusBookingAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db) => _db = db;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var totalUsers     = await _db.Users.CountAsync(u => u.Role == "Customer");
        var totalBookings  = await _db.Bookings.CountAsync();
        var confirmedBookings = await _db.Bookings.CountAsync(b => b.BookingStatus == "Confirmed");
        var cancelledBookings = await _db.Bookings.CountAsync(b => b.BookingStatus == "Cancelled");
        var totalRevenue   = await _db.Payments.Where(p => p.PaymentStatus == "Success").SumAsync(p => p.Amount);
        var totalBuses     = await _db.Buses.CountAsync(b => b.IsActive);
        var totalRoutes    = await _db.Routes.CountAsync(r => r.IsActive);
        var totalSchedules = await _db.Schedules.CountAsync(s => s.Status == "Scheduled");

        var recentBookings = await _db.Bookings
            .Include(b => b.User)
            .Include(b => b.Schedule).ThenInclude(s => s.Route)
            .OrderByDescending(b => b.BookingDate)
            .Take(5)
            .Select(b => new
            {
                b.BookingId,
                b.PNRNumber,
                b.BookingStatus,
                b.TotalAmount,
                b.BookingDate,
                UserName = b.User.FullName,
                Origin = b.Schedule.Route.Origin,
                Destination = b.Schedule.Route.Destination
            })
            .ToListAsync();

        return Ok(new
        {
            totalUsers,
            totalBookings,
            confirmedBookings,
            cancelledBookings,
            totalRevenue,
            totalBuses,
            totalRoutes,
            totalSchedules,
            recentBookings
        });
    }
}
