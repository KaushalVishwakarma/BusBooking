using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class RouteService : IRouteService
{
    private readonly AppDbContext _db;
    public RouteService(AppDbContext db) => _db = db;

    public async Task<List<RouteDto>> GetAll() =>
        await _db.Routes.Select(r => ToDto(r)).ToListAsync();

    public async Task<RouteDto?> GetById(int id) =>
        await _db.Routes.Where(r => r.RouteId == id).Select(r => ToDto(r)).FirstOrDefaultAsync();

    public async Task<RouteDto> Create(CreateRouteDto dto)
    {
        var route = new Models.Route
        {
            Origin = dto.Origin,
            Destination = dto.Destination,
            DistanceKm = dto.DistanceKm
        };
        _db.Routes.Add(route);
        await _db.SaveChangesAsync();
        return ToDto(route);
    }

    public async Task<bool> Update(int id, CreateRouteDto dto)
    {
        var route = await _db.Routes.FindAsync(id);
        if (route == null) return false;
        route.Origin = dto.Origin;
        route.Destination = dto.Destination;
        route.DistanceKm = dto.DistanceKm;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Deactivate(int id)
    {
        var route = await _db.Routes.FindAsync(id);
        if (route == null) return false;
        route.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }

    private static RouteDto ToDto(Models.Route r) => new()
    {
        RouteId = r.RouteId,
        Origin = r.Origin,
        Destination = r.Destination,
        DistanceKm = r.DistanceKm,
        IsActive = r.IsActive
    };
}
