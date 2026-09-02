using BusBookingAPI.DTOs;
using BusBookingAPI.Helpers;
using BusBookingAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _booking;
    public BookingController(IBookingService booking) => _booking = booking;

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var result = await _booking.Create(userId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyBookings()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _booking.GetUserBookings(userId));
    }

    [HttpGet("pnr/{pnr}")]
    public async Task<IActionResult> GetByPNR(string pnr)
    {
        var result = await _booking.GetByPNR(pnr);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _booking.Cancel(id, userId) ? Ok() : BadRequest("Cannot cancel this booking.");
    }

    [HttpGet("{id}/download-ticket")]
    public async Task<IActionResult> DownloadTicket(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var bookings = await _booking.GetUserBookings(userId);
        var booking = bookings.FirstOrDefault(b => b.BookingId == id);

        if (booking == null) return NotFound("Booking not found.");
        if (booking.BookingStatus != "Confirmed") return BadRequest("Ticket only available for confirmed bookings.");

        var pdf = TicketPdfGenerator.Generate(booking);
        return File(pdf, "application/pdf", $"Ticket_{booking.PNRNumber}.pdf");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _booking.GetAll());
}
