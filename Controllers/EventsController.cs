using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/events")]
public class EventsController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await db.Events.AsNoTracking().OrderBy(x => x.EventDate).Select(x => new { x.EventId, name = x.EventName, description = x.Description, date = x.EventDate, x.Location, x.Distance, x.EventType, x.Status, x.OrganiserId }).ToListAsync());

    [HttpGet("{id:int}"), AllowAnonymous]
    public async Task<IActionResult> Get(int id) { var e = await db.Events.AsNoTracking().Include(x => x.Categories).Include(x => x.WeatherRecords).FirstOrDefaultAsync(x => x.EventId == id); return e is null ? NotFound() : Ok(e); }

    [HttpPost, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(EventRequest request)
    {
        if (!new[] { "Run", "Walk", "Cycle" }.Contains(request.EventType, StringComparer.OrdinalIgnoreCase)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var e = new RaceEvent { OrganiserId = id, EventName = request.Name.Trim(), Description = request.Description, EventDate = request.Date.Date, Location = request.Location.Trim(), Distance = request.Distance, EventType = request.EventType, Status = "Available" };
        db.Events.Add(e); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = e.EventId }, e);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, EventRequest request)
    {
        var e = await db.Events.FindAsync(id); if (e is null) return NotFound();
        if (e.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        if (!new[] { "Run", "Walk", "Cycle" }.Contains(request.EventType, StringComparer.OrdinalIgnoreCase)) return BadRequest(new { message = "EventType must be Run, Walk, or Cycle." });
        e.EventName = request.Name.Trim(); e.Description = request.Description; e.EventDate = request.Date.Date; e.Location = request.Location.Trim(); e.Distance = request.Distance; e.EventType = request.EventType;
        await db.SaveChangesAsync(); return Ok(e);
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id) { var e = await db.Events.FindAsync(id); if (e is null) return NotFound(); if (e.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); db.Events.Remove(e); try { await db.SaveChangesAsync(); } catch (DbUpdateException) { return Conflict(new { message = "Event cannot be deleted while it has registrations." }); } return NoContent(); }

    [HttpGet("{eventId:int}/enrolments"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Enrolments(int eventId)
    {
        var e = await db.Events.FindAsync(eventId); if (e is null) return NotFound();
        if (e.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        return Ok(await db.Registrations.Where(r => r.EventId == eventId).Include(r => r.Participant).Include(r => r.Category).Select(r => new { r.RegistrationId, r.Status, r.RegistrationDate, participant = r.Participant!.FirstName + " " + r.Participant.LastName, r.Participant!.Email, category = r.Category!.CategoryName }).ToListAsync());
    }
}
