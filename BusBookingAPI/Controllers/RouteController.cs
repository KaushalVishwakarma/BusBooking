using BusBookingAPI.DTOs;
using BusBookingAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _route;
    public RouteController(IRouteService route) => _route = route;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _route.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _route.GetById(id);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRouteDto dto) =>
        Ok(await _route.Create(dto));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateRouteDto dto) =>
        await _route.Update(id, dto) ? Ok() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id) =>
        await _route.Deactivate(id) ? Ok() : NotFound();
}
