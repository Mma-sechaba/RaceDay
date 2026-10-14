using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/weather")]
public class WeatherController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Returns stored weather records, optionally filtered by event or location text.</summary>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] int? eventId, [FromQuery] string? location) { var q = db.WeatherRecords.AsNoTracking().Include(x => x.Event).AsQueryable(); if (eventId.HasValue) q = q.Where(x => x.EventId == eventId.Value); if (!string.IsNullOrWhiteSpace(location)) q = q.Where(x => x.Event!.Location.Contains(location)); return Ok(await q.Select(x => new { x.WeatherId, x.EventId, location = x.Event!.Location, x.Temperature, x.WeatherCondition, x.WindSpeed, x.WeatherDate }).ToListAsync()); }

    /// <summary>Returns stored forecasts for an event and optional date. This implementation uses the submitted WEATHER table and does not call a third-party live weather provider.</summary>
    [HttpGet("forecast"), AllowAnonymous]
    public async Task<IActionResult> Forecast([FromQuery] int eventId, [FromQuery] DateTime? date) { if (eventId <= 0) return BadRequest(new { message = "A valid eventId is required." }); var q = db.WeatherRecords.AsNoTracking().Where(x => x.EventId == eventId); if (date.HasValue) q = q.Where(x => x.WeatherDate == date.Value.Date); var rows = await q.ToListAsync(); if (rows.Count == 0) return NotFound(new { message = "No stored forecast is available for this event and date." }); return Ok(rows); }

    /// <summary>Stores an alert description in WEATHER. The Part 1 schema has no separate alert table, so alert data is encoded in WeatherCondition.</summary>
    [HttpPost("alerts"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateAlert(WeatherAlertRequest request) { var ev = await db.Events.FindAsync(request.EventId); if (ev is null) return NotFound(new { message = "Event does not exist." }); if (ev.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); var alertText = $"ALERT: {request.AlertType} - {request.Message}"; if (alertText.Length > 100) return BadRequest(new { message = "Alert type and message must fit within the 100-character WeatherCondition field in the Part 1 schema." }); var weather = new Weather { EventId = ev.EventId, WeatherDate = ev.EventDate, WeatherCondition = alertText }; db.WeatherRecords.Add(weather); await db.SaveChangesAsync(); return StatusCode(201, weather); }
}
