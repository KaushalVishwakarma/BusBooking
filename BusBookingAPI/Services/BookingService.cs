using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Helpers;
using BusBookingAPI.Interfaces;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    public BookingService(AppDbContext db) => _db = db;

    public async Task<BookingResponseDto> Create(int userId, CreateBookingDto dto)
    {
        var schedule = await _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .FirstOrDefaultAsync(s => s.ScheduleId == dto.ScheduleId && s.Status == "Scheduled")
            ?? throw new Exception("Schedule not found or unavailable.");

        var requestedSeatIds = dto.Passengers.Select(p => p.SeatId).ToList();

        // Check for already booked seats (concurrency-safe via DB unique constraint)
        var alreadyBooked = await _db.BookingSeats
            .Where(bs => requestedSeatIds.Contains(bs.SeatId) &&
                         bs.Booking.ScheduleId == dto.ScheduleId &&
                         bs.Booking.BookingStatus != "Cancelled")
            .AnyAsync();

        if (alreadyBooked)
            throw new Exception("One or more selected seats are already booked.");

        var totalAmount = schedule.PricePerSeat * dto.Passengers.Count;

        var booking = new Booking
        {
            UserId = userId,
            ScheduleId = dto.ScheduleId,
            TotalAmount = totalAmount,
            PNRNumber = PnrHelper.Generate(),
            BookingSeats = dto.Passengers.Select(p => new BookingSeat
            {
                SeatId = p.SeatId,
                PassengerName = p.PassengerName,
                PassengerAge = p.PassengerAge,
                PassengerGender = p.PassengerGender
            }).ToList()
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return await BuildResponse(booking.BookingId);
    }

    public async Task<List<BookingResponseDto>> GetUserBookings(int userId)
    {
        var ids = await _db.Bookings.Where(b => b.UserId == userId).Select(b => b.BookingId).ToListAsync();
        var result = new List<BookingResponseDto>();
        foreach (var id in ids) result.Add(await BuildResponse(id));
        return result;
    }

    public async Task<BookingResponseDto?> GetByPNR(string pnr)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.PNRNumber == pnr);
        return booking == null ? null : await BuildResponse(booking.BookingId);
    }

    public async Task<bool> Cancel(int bookingId, int userId)
    {
        var booking = await _db.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);
        if (booking == null || booking.BookingStatus == "Cancelled") return false;

        booking.BookingStatus = "Cancelled";

        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);
        if (payment != null) payment.PaymentStatus = "Refunded";

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<BookingResponseDto>> GetAll()
    {
        var ids = await _db.Bookings.Select(b => b.BookingId).ToListAsync();
        var result = new List<BookingResponseDto>();
        foreach (var id in ids) result.Add(await BuildResponse(id));
        return result;
    }

    private async Task<BookingResponseDto> BuildResponse(int bookingId)
    {
        var b = await _db.Bookings
            .Include(x => x.Schedule).ThenInclude(s => s.Bus)
            .Include(x => x.Schedule).ThenInclude(s => s.Route)
            .Include(x => x.BookingSeats).ThenInclude(bs => bs.Seat)
            .FirstAsync(x => x.BookingId == bookingId);

        return new BookingResponseDto
        {
            BookingId = b.BookingId,
            PNRNumber = b.PNRNumber,
            BookingStatus = b.BookingStatus,
            TotalAmount = b.TotalAmount,
            BookingDate = b.BookingDate,
            Origin = b.Schedule.Route.Origin,
            Destination = b.Schedule.Route.Destination,
            DepartureTime = b.Schedule.DepartureTime,
            BusName = b.Schedule.Bus.BusName,
            Seats = b.BookingSeats.Select(bs => new PassengerSeatDto
            {
                SeatNumber = bs.Seat.SeatNumber,
                PassengerName = bs.PassengerName,
                PassengerAge = bs.PassengerAge,
                PassengerGender = bs.PassengerGender
            }).ToList()
        };
    }
}
