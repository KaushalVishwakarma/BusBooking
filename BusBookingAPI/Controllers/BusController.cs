using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusController : ControllerBase
{
    private readonly IBusService _bus;
    public BusController(IBusService bus) => _bus = bus;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _bus.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bus.GetById(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/seats")]
    public async Task<IActionResult> GetSeats(int id, [FromQuery] int scheduleId) =>
        Ok(await _bus.GetSeats(id, scheduleId));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBusDto dto)
    {
        var result = await _bus.Create(dto);
        if (result == null) return Conflict(new { message = "Bus number already exists." });
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/seats")]
    public async Task<IActionResult> AddSeats(int id, List<CreateSeatDto> seats)
    {
        await _bus.AddSeats(id, seats);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateBusDto dto) =>
        await _bus.Update(id, dto) ? Ok() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id) =>
        await _bus.Deactivate(id) ? Ok() : NotFound();
}
