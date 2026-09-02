namespace BusBookingAPI.Models;

public class BookingSeat
{
    public int BookingSeatId { get; set; }
    public int BookingId { get; set; }
    public int SeatId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public int PassengerAge { get; set; }
    public string PassengerGender { get; set; } = string.Empty;  // Male | Female | Other

    public Booking Booking { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
}
