using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Security;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/profile"), Authorize]
public class ProfileController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (User.IsInRole("Organiser")) { var o = await db.Organisers.AsNoTracking().Where(x => x.OrganiserId == id).Select(x => new { userId = x.OrganiserId, x.FirstName, x.LastName, x.Email, x.Phone, role = "Organiser" }).FirstOrDefaultAsync(); return o is null ? NotFound() : Ok(o); }
        var p = await db.Participants.AsNoTracking().Where(x => x.ParticipantId == id).Select(x => new { userId = x.ParticipantId, x.FirstName, x.LastName, x.Email, x.Phone, role = "Participant", x.DateOfBirth }).FirstOrDefaultAsync(); return p is null ? NotFound() : Ok(p);
    }

    [HttpPut]
    public async Task<IActionResult> Update(ProfileUpdateRequest request)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (await db.Organisers.AnyAsync(x => x.Email == request.Email && (!User.IsInRole("Organiser") || x.OrganiserId != id)) || await db.Participants.AnyAsync(x => x.Email == request.Email && (!User.IsInRole("Participant") || x.ParticipantId != id))) return Conflict(new { message = "Email already in use." });
        if (User.IsInRole("Organiser")) { var o = await db.Organisers.FindAsync(id); if (o is null) return NotFound(); o.FirstName = request.FirstName.Trim(); o.LastName = request.LastName.Trim(); o.Email = request.Email.Trim().ToLowerInvariant(); o.Phone = request.Phone; }
        else { var p = await db.Participants.FindAsync(id); if (p is null) return NotFound(); p.FirstName = request.FirstName.Trim(); p.LastName = request.LastName.Trim(); p.Email = request.Email.Trim().ToLowerInvariant(); p.Phone = request.Phone; }
        await db.SaveChangesAsync(); HttpContext.Session.SetString("Email", request.Email.Trim().ToLowerInvariant()); HttpContext.Session.SetString("FullName", $"{request.FirstName} {request.LastName}"); return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (User.IsInRole("Organiser")) { var o = await db.Organisers.FindAsync(id); if (o is null) return NotFound(); if (!PasswordService.Verify(request.CurrentPassword, o.Password)) return Unauthorized(new { message = "Current password is incorrect." }); o.Password = PasswordService.Hash(request.NewPassword); }
        else { var p = await db.Participants.FindAsync(id); if (p is null) return NotFound(); if (!PasswordService.Verify(request.CurrentPassword, p.Password)) return Unauthorized(new { message = "Current password is incorrect." }); p.Password = PasswordService.Hash(request.NewPassword); }
        await db.SaveChangesAsync(); return Ok(new { message = "Password changed successfully." });
    }
}
