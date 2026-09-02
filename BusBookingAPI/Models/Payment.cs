namespace BusBookingAPI.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;   // Stripe | Card | UPI
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = "Pending";      // Pending | Success | Failed | Refunded
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public Booking Booking { get; set; } = null!;
}
