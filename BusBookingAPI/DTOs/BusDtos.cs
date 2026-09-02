namespace BusBookingAPI.DTOs;

public class BusDto
{
    public int BusId { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBusDto
{
    public string BusNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
}

public class SeatDto
{
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatType { get; set; } = string.Empty;
    public string Deck { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
}

public class CreateSeatDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatType { get; set; } = string.Empty;
    public string Deck { get; set; } = "Lower";
}
