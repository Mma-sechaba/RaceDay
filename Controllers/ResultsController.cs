using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/results")]
public class ResultsController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await db.Results.AsNoTracking().Include(x => x.Registration).ThenInclude(x => x!.Participant).Include(x => x.Registration).ThenInclude(x => x!.Event).Include(x => x.Registration).ThenInclude(x => x!.Category).Select(x => new { x.ResultId, x.FinishTime, x.Position, x.RegistrationId, participant = x.Registration!.Participant!.FirstName + " " + x.Registration.Participant.LastName, eventName = x.Registration.Event!.EventName, category = x.Registration.Category!.CategoryName }).ToListAsync());

    [HttpGet("my"), Authorize(Roles = "Participant")]
    public async Task<IActionResult> MyResults() { var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); return Ok(await db.Results.AsNoTracking().Include(x => x.Registration).ThenInclude(x => x!.Event).Include(x => x.Registration).ThenInclude(x => x!.Category).Where(x => x.Registration!.ParticipantId == id).Select(x => new { x.ResultId, x.FinishTime, x.Position, eventName = x.Registration!.Event!.EventName, eventDate = x.Registration.Event.EventDate, category = x.Registration.Category!.CategoryName }).ToListAsync()); }

    [HttpGet("{id:int}"), Authorize]
    public async Task<IActionResult> Get(int id) { var r = await db.Results.AsNoTracking().Include(x => x.Registration).ThenInclude(x => x!.Event).Include(x => x.Registration).ThenInclude(x => x!.Participant).FirstOrDefaultAsync(x => x.ResultId == id); if (r is null) return NotFound(); var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); if (User.IsInRole("Participant") && r.Registration!.ParticipantId != userId) return Forbid(); if (User.IsInRole("Organiser") && r.Registration!.Event!.OrganiserId != userId) return Forbid(); return Ok(new { r.ResultId, r.RegistrationId, r.FinishTime, r.Position, participantName = r.Registration!.Participant!.FirstName + " " + r.Registration.Participant.LastName, eventName = r.Registration.Event!.EventName }); }

    [HttpPost, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(ResultRequest request) { var registration = await db.Registrations.Include(x => x.Event).FirstOrDefaultAsync(x => x.RegistrationId == request.RegistrationId); if (registration is null) return NotFound(new { message = "Registration does not exist." }); if (registration.Event!.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); if (await db.Results.AnyAsync(x => x.RegistrationId == request.RegistrationId)) return Conflict(new { message = "A result already exists for this registration." }); if (request.FinishTime < TimeSpan.Zero || request.Position < 1) return BadRequest(); var result = new Result { RegistrationId = request.RegistrationId, FinishTime = request.FinishTime, Position = request.Position }; db.Results.Add(result); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = result.ResultId }, result); }

    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, ResultUpdateRequest request) { var r = await db.Results.Include(x => x.Registration).ThenInclude(x => x!.Event).FirstOrDefaultAsync(x => x.ResultId == id); if (r is null) return NotFound(); if (r.Registration!.Event!.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); if (request.FinishTime < TimeSpan.Zero || request.Position < 1) return BadRequest(); r.FinishTime = request.FinishTime; r.Position = request.Position; await db.SaveChangesAsync(); return Ok(r); }

    [HttpDelete("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id) { var r = await db.Results.Include(x => x.Registration).ThenInclude(x => x!.Event).FirstOrDefaultAsync(x => x.ResultId == id); if (r is null) return NotFound(); if (r.Registration!.Event!.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); db.Results.Remove(r); await db.SaveChangesAsync(); return NoContent(); }
}
