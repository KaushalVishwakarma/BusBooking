namespace BusBookingAPI.DTOs;

public class ScheduleDto
{
    public int ScheduleId { get; set; }
    public int BusId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public int RouteId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal PricePerSeat { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
}

public class CreateScheduleDto
{
    public int BusId { get; set; }
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal PricePerSeat { get; set; }
}

public class SearchScheduleDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateOnly TravelDate { get; set; }
}
