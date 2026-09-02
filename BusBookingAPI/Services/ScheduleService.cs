using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;
    public ScheduleService(AppDbContext db) => _db = db;

    public async Task<List<ScheduleDto>> Search(SearchScheduleDto dto)
    {
        if (dto.TravelDate < DateOnly.FromDateTime(DateTime.Today))
            return new List<ScheduleDto>();

        return await _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Bookings).ThenInclude(b => b.BookingSeats)
            .Where(s =>
                s.Route.Origin.ToLower() == dto.Origin.ToLower() &&
                s.Route.Destination.ToLower() == dto.Destination.ToLower() &&
                s.DepartureTime.Year == dto.TravelDate.Year &&
                s.DepartureTime.Month == dto.TravelDate.Month &&
                s.DepartureTime.Day == dto.TravelDate.Day &&
                s.Status == "Scheduled")
            .Select(s => ToDto(s))
            .ToListAsync();
    }

    public async Task<ScheduleDto?> GetById(int id) =>
        await _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Where(s => s.ScheduleId == id)
            .Select(s => ToDto(s))
            .FirstOrDefaultAsync();

    public async Task<List<ScheduleDto>> GetAll() =>
        await _db.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Select(s => ToDto(s))
            .ToListAsync();

    public async Task<ScheduleDto> Create(CreateScheduleDto dto)
    {
        var schedule = new Schedule
        {
            BusId = dto.BusId,
            RouteId = dto.RouteId,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            PricePerSeat = dto.PricePerSeat
        };
        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();

        await _db.Entry(schedule).Reference(s => s.Bus).LoadAsync();
        await _db.Entry(schedule).Reference(s => s.Route).LoadAsync();
        return ToDto(schedule);
    }

    public async Task<bool> UpdateStatus(int id, string status)
    {
        var schedule = await _db.Schedules.FindAsync(id);
        if (schedule == null) return false;
        schedule.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    private static ScheduleDto ToDto(Schedule s)
    {
        var bookedSeats = s.Bookings?.Where(b => b.BookingStatus != "Cancelled")
            .SelectMany(b => b.BookingSeats).Count() ?? 0;

        return new ScheduleDto
        {
            ScheduleId = s.ScheduleId,
            BusId = s.BusId,
            BusName = s.Bus.BusName,
            BusNumber = s.Bus.BusNumber,
            BusType = s.Bus.BusType,
            RouteId = s.RouteId,
            Origin = s.Route.Origin,
            Destination = s.Route.Destination,
            DepartureTime = s.DepartureTime,
            ArrivalTime = s.ArrivalTime,
            PricePerSeat = s.PricePerSeat,
            Status = s.Status,
            AvailableSeats = s.Bus.TotalSeats - bookedSeats
        };
    }
}
