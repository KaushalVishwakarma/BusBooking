namespace BusBookingAPI.DTOs;

public class RouteDto
{
    public int RouteId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRouteDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
}
