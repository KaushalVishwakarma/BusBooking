namespace BusBookingAPI.DTOs;

public class CreatePaymentDto
{
    public int BookingId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayKeyId { get; set; } = string.Empty;
}

public class ConfirmPaymentDto
{
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public string RazorpaySignature { get; set; } = string.Empty;
}
