using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/organisers")]
public class OrganisersController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, AllowAnonymous] public async Task<IActionResult> GetAll() => Ok(await db.Organisers.AsNoTracking().Select(x => new { x.OrganiserId, x.FirstName, x.LastName, x.Email, x.Phone }).ToListAsync());
    [HttpGet("{id:int}"), AllowAnonymous] public async Task<IActionResult> Get(int id) { var o = await db.Organisers.AsNoTracking().Include(x => x.Events).Where(x => x.OrganiserId == id).Select(x => new { x.OrganiserId, x.FirstName, x.LastName, x.Email, x.Phone, events = x.Events.Select(e => new { e.EventId, e.EventName, e.EventDate, e.Location }) }).FirstOrDefaultAsync(); return o is null ? NotFound() : Ok(o); }
    [HttpPost("Create"), AllowAnonymous]
    public async Task<IActionResult> Create(OrganiserCreateRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Organisers.AnyAsync(x => x.Email.ToLower() == email) || await db.Participants.AnyAsync(x => x.Email.ToLower() == email)) return Conflict(new { message = "Email already registered." });
        var o = new EventOrganiser { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, Password = PasswordService.Hash(request.Password), Phone = request.Phone };
        db.Organisers.Add(o); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = o.OrganiserId }, new { o.OrganiserId, o.FirstName, o.LastName, o.Email, o.Phone });
    }
    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, ProfileUpdateRequest request) { var current = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); if (id != current) return Forbid(); var o = await db.Organisers.FindAsync(id); if (o is null) return NotFound(); var email = request.Email.Trim().ToLowerInvariant(); if (await db.Participants.AnyAsync(x => x.Email.ToLower() == email) || await db.Organisers.AnyAsync(x => x.Email.ToLower() == email && x.OrganiserId != id)) return Conflict(new { message = "Email already in use." }); o.FirstName = request.FirstName.Trim(); o.LastName = request.LastName.Trim(); o.Email = email; o.Phone = request.Phone; await db.SaveChangesAsync(); HttpContext.Session.SetString("Email", o.Email); HttpContext.Session.SetString("FullName", $"{o.FirstName} {o.LastName}"); return Ok(new { o.OrganiserId, o.FirstName, o.LastName, o.Email, o.Phone }); }
    [HttpDelete("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id) { if (id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); var o = await db.Organisers.FindAsync(id); if (o is null) return NotFound(); db.Organisers.Remove(o); try { await db.SaveChangesAsync(); } catch (DbUpdateException) { return Conflict(new { message = "Organiser cannot be deleted while events belong to this account." }); } HttpContext.Session.Clear(); return NoContent(); }
}
