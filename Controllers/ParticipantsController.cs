using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/participants")]
public class ParticipantsController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Organiser")] public async Task<IActionResult> GetAll() => Ok(await db.Participants.AsNoTracking().Select(x => new { x.ParticipantId, x.FirstName, x.LastName, x.Email, x.DateOfBirth, x.Phone }).ToListAsync());
    [HttpGet("{id:int}"), Authorize]
    public async Task<IActionResult> Get(int id) { var own = User.IsInRole("Participant") && id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); if (!own && !User.IsInRole("Organiser")) return Forbid(); var p = await db.Participants.AsNoTracking().Where(x => x.ParticipantId == id).Select(x => new { x.ParticipantId, x.FirstName, x.LastName, x.Email, x.DateOfBirth, x.Phone }).FirstOrDefaultAsync(); return p is null ? NotFound() : Ok(p); }
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Create(ParticipantCreateRequest request) { var email = request.Email.Trim().ToLowerInvariant(); if (await db.Participants.AnyAsync(x => x.Email.ToLower() == email) || await db.Organisers.AnyAsync(x => x.Email.ToLower() == email)) return Conflict(new { message = "Email already registered." }); var p = new Participant { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, Password = PasswordService.Hash(request.Password), DateOfBirth = request.DateOfBirth.Date, Phone = request.Phone }; db.Participants.Add(p); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = p.ParticipantId }, new { p.ParticipantId, p.FirstName, p.LastName, p.Email, p.DateOfBirth, p.Phone }); }
    [HttpPut("{id:int}"), Authorize(Roles = "Participant")]
    public async Task<IActionResult> Update(int id, ProfileUpdateRequest request) { if (id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); var p = await db.Participants.FindAsync(id); if (p is null) return NotFound(); if (await db.Participants.AnyAsync(x => x.Email == request.Email && x.ParticipantId != id) || await db.Organisers.AnyAsync(x => x.Email == request.Email)) return Conflict(new { message = "Email already in use." }); p.FirstName = request.FirstName.Trim(); p.LastName = request.LastName.Trim(); p.Email = request.Email.Trim().ToLowerInvariant(); p.Phone = request.Phone; await db.SaveChangesAsync(); HttpContext.Session.SetString("Email", p.Email); HttpContext.Session.SetString("FullName", $"{p.FirstName} {p.LastName}"); return Ok(new { p.ParticipantId, p.FirstName, p.LastName, p.Email, p.DateOfBirth, p.Phone }); }
    [HttpDelete("{id:int}"), Authorize]
    public async Task<IActionResult> Delete(int id) { var own = User.IsInRole("Participant") && id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); if (!own && !User.IsInRole("Organiser")) return Forbid(); var p = await db.Participants.FindAsync(id); if (p is null) return NotFound(); db.Participants.Remove(p); try { await db.SaveChangesAsync(); } catch (DbUpdateException) { return Conflict(new { message = "Participant cannot be deleted while registrations exist." }); } if (own) HttpContext.Session.Clear(); return NoContent(); }
}
