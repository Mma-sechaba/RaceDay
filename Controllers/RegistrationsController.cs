using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/registrations")]
public class RegistrationsController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetAll() => Ok(await db.Registrations.AsNoTracking().Include(x => x.Participant).Include(x => x.Event).Include(x => x.Category).Where(x => x.Event!.OrganiserId == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).Select(x => new { x.RegistrationId, x.ParticipantId, x.EventId, x.CategoryId, x.RegistrationDate, x.Status, participantName = x.Participant!.FirstName + " " + x.Participant.LastName, eventName = x.Event!.EventName, categoryName = x.Category!.CategoryName }).ToListAsync());

    [HttpGet("{id:int}"), Authorize]
    public async Task<IActionResult> Get(int id) { var r = await db.Registrations.AsNoTracking().Include(x => x.Event).Include(x => x.Participant).Include(x => x.Category).FirstOrDefaultAsync(x => x.RegistrationId == id); if (r is null) return NotFound(); if (!CanAccess(r)) return Forbid(); return Ok(new { r.RegistrationId, r.ParticipantId, r.EventId, r.CategoryId, r.RegistrationDate, r.Status, participantName = r.Participant!.FirstName + " " + r.Participant.LastName, eventName = r.Event!.EventName, categoryName = r.Category!.CategoryName }); }

    [HttpPost, Authorize(Roles = "Participant")]
    public async Task<IActionResult> Create(RegistrationRequest request)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await db.Events.FindAsync(request.EventId); if (ev is null) return NotFound(new { message = "Event does not exist." });
        var category = await db.Categories.FindAsync(request.CategoryId); if (category is null || category.EventId != request.EventId) return NotFound(new { message = "Category does not exist for the selected event." });
        if (await db.Registrations.AnyAsync(x => x.ParticipantId == participantId && x.EventId == request.EventId)) return Conflict(new { message = "Participant is already registered for this event." });
        var r = new Registration { ParticipantId = participantId, EventId = request.EventId, CategoryId = request.CategoryId, Status = "Pending" };
        db.Registrations.Add(r); await db.SaveChangesAsync(); return StatusCode(201, r);
    }

    [HttpPut("{id:int}"), Authorize]
    public async Task<IActionResult> Update(int id, RegistrationUpdateRequest request)
    {
        var r = await db.Registrations.Include(x => x.Event).FirstOrDefaultAsync(x => x.RegistrationId == id); if (r is null) return NotFound();
        if (!CanAccess(r)) return Forbid();
        if (request.CategoryId.HasValue) { var category = await db.Categories.FindAsync(request.CategoryId.Value); if (category is null || category.EventId != r.EventId) return BadRequest(new { message = "Category must belong to the registered event." }); r.CategoryId = request.CategoryId.Value; }
        if (request.Status is not null) { if (!User.IsInRole("Organiser")) return Forbid(); if (!new[] { "Pending", "Confirmed", "Cancelled" }.Contains(request.Status, StringComparer.OrdinalIgnoreCase)) return BadRequest(new { message = "Status must be Pending, Confirmed, or Cancelled." }); r.Status = request.Status; }
        await db.SaveChangesAsync(); return Ok(r);
    }

    [HttpDelete("{id:int}"), Authorize]
    public async Task<IActionResult> Delete(int id) { var r = await db.Registrations.Include(x => x.Event).FirstOrDefaultAsync(x => x.RegistrationId == id); if (r is null) return NotFound(); if (!CanAccess(r)) return Forbid(); db.Registrations.Remove(r); await db.SaveChangesAsync(); return NoContent(); }

    [HttpPost("/api/enrolments"), Authorize(Roles = "Participant")]
    public Task<IActionResult> Enrol([FromBody] RegistrationRequest request) => Create(request);
    [HttpDelete("/api/enrolments/{id:int}"), Authorize]
    public Task<IActionResult> CancelEnrolment(int id) => Delete(id);

    private bool CanAccess(Registration r)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return User.IsInRole("Organiser") ? r.Event?.OrganiserId == id : User.IsInRole("Participant") && r.ParticipantId == id;
    }
}
