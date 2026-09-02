using BusBookingAPI.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BusBookingAPI.Helpers;

public static class TicketPdfGenerator
{
    public static byte[] Generate(BookingResponseDto booking)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Background("#1a73e8").Padding(20).Row(row =>
                    {
                        row.RelativeItem().Text("🚌 Bus Booking E-Ticket")
                            .FontSize(22).FontColor("#ffffff").Bold();
                        row.ConstantItem(150).AlignRight().Text($"PNR: {booking.PNRNumber}")
                            .FontSize(13).FontColor("#ffffff").Bold();
                    });

                    col.Item().Height(20);

                    // Booking Info
                    col.Item().Border(1).BorderColor("#dddddd").Padding(15).Column(info =>
                    {
                        info.Item().Text("Booking Details").FontSize(14).Bold().FontColor("#1a73e8");
                        info.Item().Height(8);

                        info.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Bus: {booking.BusName}").SemiBold();
                                c.Item().Text($"From: {booking.Origin}");
                                c.Item().Text($"To: {booking.Destination}");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Departure: {booking.DepartureTime:dd MMM yyyy, hh:mm tt}").SemiBold();
                                c.Item().Text($"Booking Date: {booking.BookingDate:dd MMM yyyy}");
                                c.Item().Text($"Status: {booking.BookingStatus}").FontColor("#2e7d32");
                            });
                        });
                    });

                    col.Item().Height(15);

                    // Passenger Details
                    col.Item().Border(1).BorderColor("#dddddd").Padding(15).Column(pass =>
                    {
                        pass.Item().Text("Passenger Details").FontSize(14).Bold().FontColor("#1a73e8");
                        pass.Item().Height(8);

                        // Table Header
                        pass.Item().Background("#f5f5f5").Padding(8).Row(row =>
                        {
                            row.RelativeItem().Text("Seat No").Bold();
                            row.RelativeItem().Text("Passenger Name").Bold();
                            row.RelativeItem().Text("Age").Bold();
                            row.RelativeItem().Text("Gender").Bold();
                        });

                        // Table Rows
                        foreach (var seat in booking.Seats)
                        {
                            pass.Item().BorderBottom(1).BorderColor("#eeeeee").Padding(8).Row(row =>
                            {
                                row.RelativeItem().Text(seat.SeatNumber);
                                row.RelativeItem().Text(seat.PassengerName);
                                row.RelativeItem().Text(seat.PassengerAge.ToString());
                                row.RelativeItem().Text(seat.PassengerGender);
                            });
                        }
                    });

                    col.Item().Height(15);

                    // Payment Summary
                    col.Item().Border(1).BorderColor("#dddddd").Padding(15).Column(pay =>
                    {
                        pay.Item().Text("Payment Summary").FontSize(14).Bold().FontColor("#1a73e8");
                        pay.Item().Height(8);
                        pay.Item().AlignRight().Text($"Total Amount Paid: ₹{booking.TotalAmount}")
                            .FontSize(16).Bold().FontColor("#2e7d32");
                    });

                    col.Item().Height(20);

                    // Footer
                    col.Item().AlignCenter().Text("Thank you for booking with us! Have a safe journey. 🙏")
                        .FontSize(11).FontColor("#888888").Italic();
                });
            });
        }).GeneratePdf();
    }
}
