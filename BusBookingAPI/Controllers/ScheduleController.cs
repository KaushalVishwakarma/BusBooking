using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _schedule;
    public ScheduleController(IScheduleService schedule) => _schedule = schedule;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchScheduleDto dto) =>
        Ok(await _schedule.Search(dto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _schedule.GetById(id);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _schedule.GetAll());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateScheduleDto dto) =>
        Ok(await _schedule.Create(dto));

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status) =>
        await _schedule.UpdateStatus(id, status) ? Ok() : NotFound();
}
