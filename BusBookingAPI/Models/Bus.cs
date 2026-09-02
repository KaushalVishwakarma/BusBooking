namespace BusBookingAPI.Models;

public class Bus
{
    public int BusId { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;  // Sleeper | Semi-Sleeper | Seater
    public int TotalSeats { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
