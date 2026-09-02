namespace BusBookingAPI.Models;

public class Seat
{
    public int SeatId { get; set; }
    public int BusId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;  // A1, A2, B1
    public string SeatType { get; set; } = string.Empty;    // Window | Aisle | Middle
    public string Deck { get; set; } = "Lower";             // Lower | Upper

    public Bus Bus { get; set; } = null!;
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}
