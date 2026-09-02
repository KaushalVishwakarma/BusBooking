using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _payment;
    public PaymentController(IPaymentService payment) => _payment = payment;

    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate(CreatePaymentDto dto)
    {
        try
        {
            var result = await _payment.InitiatePayment(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(ConfirmPaymentDto dto) =>
        await _payment.ConfirmPayment(dto) ? Ok() : BadRequest("Payment verification failed.");

    [HttpPost("refund/{bookingId}")]
    public async Task<IActionResult> Refund(int bookingId) =>
        await _payment.RefundPayment(bookingId) ? Ok() : BadRequest("Refund failed.");
}
