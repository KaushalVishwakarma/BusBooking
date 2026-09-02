namespace BusBookingAPI.DTOs;

public class PassengerDto
{
    public int SeatId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public int PassengerAge { get; set; }
    public string PassengerGender { get; set; } = string.Empty;
}

public class CreateBookingDto
{
    public int ScheduleId { get; set; }
    public List<PassengerDto> Passengers { get; set; } = new();
}

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public string PNRNumber { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string BusName { get; set; } = string.Empty;
    public List<PassengerSeatDto> Seats { get; set; } = new();
}

public class PassengerSeatDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public int PassengerAge { get; set; }
    public string PassengerGender { get; set; } = string.Empty;
}
