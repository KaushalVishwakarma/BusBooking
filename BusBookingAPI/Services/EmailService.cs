using BusBookingAPI.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BusBookingAPI.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendTicketAsync(string toEmail, string toName, BookingResponseDto booking, byte[] pdfBytes)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["Email:SenderName"], _config["Email:SenderEmail"]));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = $"Your Bus Ticket - PNR: {booking.PNRNumber}";

        var builder = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='font-family:Arial,sans-serif; max-width:600px; margin:auto;'>
                    <div style='background:#1a73e8; padding:20px; text-align:center;'>
                        <h1 style='color:white; margin:0;'>🚌 Booking Confirmed!</h1>
                    </div>
                    <div style='padding:20px; background:#f9f9f9;'>
                        <p>Dear <strong>{toName}</strong>,</p>
                        <p>Your bus ticket has been booked successfully. Please find your e-ticket attached.</p>
                        <table style='width:100%; border-collapse:collapse; margin:15px 0;'>
                            <tr style='background:#e8f0fe;'>
                                <td style='padding:10px; font-weight:bold;'>PNR Number</td>
                                <td style='padding:10px;'>{booking.PNRNumber}</td>
                            </tr>
                            <tr>
                                <td style='padding:10px; font-weight:bold;'>From</td>
                                <td style='padding:10px;'>{booking.Origin}</td>
                            </tr>
                            <tr style='background:#e8f0fe;'>
                                <td style='padding:10px; font-weight:bold;'>To</td>
                                <td style='padding:10px;'>{booking.Destination}</td>
                            </tr>
                            <tr>
                                <td style='padding:10px; font-weight:bold;'>Departure</td>
                                <td style='padding:10px;'>{booking.DepartureTime:dd MMM yyyy, hh:mm tt}</td>
                            </tr>
                            <tr style='background:#e8f0fe;'>
                                <td style='padding:10px; font-weight:bold;'>Bus</td>
                                <td style='padding:10px;'>{booking.BusName}</td>
                            </tr>
                            <tr>
                                <td style='padding:10px; font-weight:bold;'>Total Amount</td>
                                <td style='padding:10px; color:green; font-weight:bold;'>₹{booking.TotalAmount}</td>
                            </tr>
                        </table>
                        <p style='color:#888; font-size:12px;'>Please carry this ticket (printed or on your phone) during your journey.</p>
                    </div>
                    <div style='background:#1a73e8; padding:10px; text-align:center;'>
                        <p style='color:white; margin:0; font-size:12px;'>Have a safe journey! 🙏</p>
                    </div>
                </div>"
        };

        builder.Attachments.Add($"Ticket_{booking.PNRNumber}.pdf", pdfBytes, ContentType.Parse("application/pdf"));
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]!), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Email:SenderEmail"], _config["Email:SenderPassword"]);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
