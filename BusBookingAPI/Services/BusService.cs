using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class BusService : IBusService
{
    private readonly AppDbContext _db;
    public BusService(AppDbContext db) => _db = db;

    public async Task<List<BusDto>> GetAll() =>
        await _db.Buses.Select(b => ToDto(b)).ToListAsync();

    public async Task<BusDto?> GetById(int id) =>
        await _db.Buses.Where(b => b.BusId == id).Select(b => ToDto(b)).FirstOrDefaultAsync();

    public async Task<BusDto?> Create(CreateBusDto dto)
    {
        if (await _db.Buses.AnyAsync(b => b.BusNumber == dto.BusNumber))
            return null;

        var bus = new Bus
        {
            BusNumber = dto.BusNumber,
            BusName = dto.BusName,
            BusType = dto.BusType,
            TotalSeats = dto.TotalSeats,
            Seats = GenerateSeatsForBus(dto.BusType, dto.TotalSeats)
        };
        _db.Buses.Add(bus);
        await _db.SaveChangesAsync();
        return ToDto(bus);
    }

    public async Task<bool> Update(int id, CreateBusDto dto)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus == null) return false;
        bus.BusNumber = dto.BusNumber;
        bus.BusName = dto.BusName;
        bus.BusType = dto.BusType;
        bus.TotalSeats = dto.TotalSeats;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Deactivate(int id)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus == null) return false;
        _db.Buses.Remove(bus);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SeatDto>> GetSeats(int busId, int scheduleId)
    {
        var bookedSeatIds = await _db.BookingSeats
            .Where(bs => bs.Booking.ScheduleId == scheduleId &&
                         bs.Booking.BookingStatus != "Cancelled")
            .Select(bs => bs.SeatId)
            .ToListAsync();

        return await _db.Seats
            .Where(s => s.BusId == busId)
            .Select(s => new SeatDto
            {
                SeatId = s.SeatId,
                SeatNumber = s.SeatNumber,
                SeatType = s.SeatType,
                Deck = s.Deck,
                IsBooked = bookedSeatIds.Contains(s.SeatId)
            }).ToListAsync();
    }

    public async Task AddSeats(int busId, List<CreateSeatDto> seats)
    {
        var entities = seats.Select(s => new Seat
        {
            BusId = busId,
            SeatNumber = s.SeatNumber,
            SeatType = s.SeatType,
            Deck = s.Deck
        });
        _db.Seats.AddRange(entities);
        await _db.SaveChangesAsync();
    }

    private static BusDto ToDto(Bus b) => new()
    {
        BusId = b.BusId,
        BusNumber = b.BusNumber,
        BusName = b.BusName,
        BusType = b.BusType,
        TotalSeats = b.TotalSeats,
        IsActive = b.IsActive
    };

    private static List<Seat> GenerateSeatsForBus(string busType, int totalSeats)
    {
        var seats = new List<Seat>();

        if (busType == "Sleeper")
        {
            int perDeck = totalSeats / 2;
            foreach (var deck in new[] { "Lower", "Upper" })
            {
                string prefix = deck == "Lower" ? "L" : "U";
                int rows = perDeck / 2;
                for (int r = 1; r <= rows; r++)
                {
                    seats.Add(new Seat { SeatNumber = $"{prefix}L{r}", SeatType = "Window", Deck = deck });
                    seats.Add(new Seat { SeatNumber = $"{prefix}R{r}", SeatType = "Window", Deck = deck });
                }
            }
        }
        else
        {
            int rows = totalSeats / 4;
            string[] cols = ["A", "B", "C", "D"];
            for (int r = 1; r <= rows; r++)
                for (int c = 0; c < 4; c++)
                    seats.Add(new Seat
                    {
                        SeatNumber = $"{cols[c]}{r}",
                        SeatType = c == 0 || c == 3 ? "Window" : "Aisle",
                        Deck = "Lower"
                    });
        }

        return seats;
    }
}
