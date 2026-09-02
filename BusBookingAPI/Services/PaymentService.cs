using System.Security.Cryptography;
using System.Text;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Helpers;
using BusBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using PaymentModel = BusBookingAPI.Models.Payment;

namespace BusBookingAPI.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;
    private readonly EmailService _email;
    private readonly IBookingService _booking;
    private readonly string _keyId;
    private readonly string _keySecret;

    public PaymentService(AppDbContext db, IConfiguration config, EmailService email, IBookingService booking)
    {
        _db = db;
        _email = email;
        _booking = booking;
        _keyId = config["Razorpay:KeyId"]!;
        _keySecret = config["Razorpay:KeySecret"]!;
    }

    public async Task<PaymentResponseDto> InitiatePayment(CreatePaymentDto dto)
    {
        var booking = await _db.Bookings.FindAsync(dto.BookingId)
            ?? throw new Exception("Booking not found.");

        var client = new RazorpayClient(_keyId, _keySecret);
        var options = new Dictionary<string, object>
        {
            { "amount", (long)(booking.TotalAmount * 100) },
            { "currency", "INR" },
            { "receipt", $"booking_{booking.BookingId}" }
        };
        var order = client.Order.Create(options);
        string orderId = order["id"].ToString();

        var payment = new PaymentModel
        {
            BookingId = dto.BookingId,
            Amount = booking.TotalAmount,
            PaymentMethod = dto.PaymentMethod,
            TransactionId = orderId,
            PaymentStatus = "Pending"
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        return new PaymentResponseDto
        {
            PaymentId = payment.PaymentId,
            TransactionId = orderId,
            PaymentStatus = payment.PaymentStatus,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            RazorpayOrderId = orderId,
            RazorpayKeyId = _keyId
        };
    }

    public async Task<bool> ConfirmPayment(ConfirmPaymentDto dto)
    {
        var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

        if (generatedSignature != dto.RazorpaySignature)
            return false;

        var payment = await _db.Payments
            .Include(p => p.Booking).ThenInclude(b => b.User)
            .FirstOrDefaultAsync(p => p.TransactionId == dto.RazorpayOrderId);

        if (payment == null) return false;

        payment.PaymentStatus = "Success";
        payment.Booking.BookingStatus = "Confirmed";
        await _db.SaveChangesAsync();

        var bookingDto = await _booking.GetByPNR(payment.Booking.PNRNumber);
        if (bookingDto != null)
        {
            var pdf = TicketPdfGenerator.Generate(bookingDto);
            await _email.SendTicketAsync(payment.Booking.User.Email, payment.Booking.User.FullName, bookingDto, pdf);
        }

        return true;
    }

    public async Task<bool> RefundPayment(int bookingId)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.PaymentStatus == "Success");

        if (payment == null) return false;

        var client = new RazorpayClient(_keyId, _keySecret);
        var refundOptions = new Dictionary<string, object>
        {
            { "amount", (long)(payment.Amount * 100) }
        };
        client.Payment.Fetch(payment.TransactionId).Refund(refundOptions);

        payment.PaymentStatus = "Refunded";
        await _db.SaveChangesAsync();
        return true;
    }
}
