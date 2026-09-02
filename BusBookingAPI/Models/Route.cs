namespace BusBookingAPI.Models;

public class Route
{
    public int RouteId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
