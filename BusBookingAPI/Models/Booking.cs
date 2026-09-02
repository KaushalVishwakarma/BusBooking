namespace BusBookingAPI.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int ScheduleId { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = "Pending";  // Pending | Confirmed | Cancelled
    public string PNRNumber { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public Schedule Schedule { get; set; } = null!;
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public Payment? Payment { get; set; }
}
