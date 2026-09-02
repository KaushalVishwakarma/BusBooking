namespace BusBookingAPI.Models;

public class Schedule
{
    public int ScheduleId { get; set; }
    public int BusId { get; set; }
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal PricePerSeat { get; set; }
    public string Status { get; set; } = "Scheduled";  // Scheduled | Cancelled | Completed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
    public Route Route { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
