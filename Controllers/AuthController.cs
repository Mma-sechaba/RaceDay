using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RaceDayDbContext db) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var role = request.Role.Trim();
        if (role.Equals("Organiser", StringComparison.OrdinalIgnoreCase) || role.Equals("Event Organiser", StringComparison.OrdinalIgnoreCase)) role = "Organiser";
        else if (role.Equals("Participant", StringComparison.OrdinalIgnoreCase)) role = "Participant";
        else return BadRequest(new { message = "Role must be Participant or Organiser." });
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Organisers.AnyAsync(x => x.Email.ToLower() == email) || await db.Participants.AnyAsync(x => x.Email.ToLower() == email))
            return Conflict(new { message = "Email already registered." });
        if (role == "Participant" && request.DateOfBirth is null) return BadRequest(new { message = "DateOfBirth is required for a Participant." });
        if (role == "Organiser")
        {
            var user = new EventOrganiser { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, Password = PasswordService.Hash(request.Password), Phone = request.Phone };
            db.Organisers.Add(user); await db.SaveChangesAsync();
            return StatusCode(201, new { userId = user.OrganiserId, user.FirstName, user.LastName, user.Email, role });
        }
        var participant = new Participant { FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(), Email = email, Password = PasswordService.Hash(request.Password), DateOfBirth = request.DateOfBirth!.Value.Date, Phone = request.Phone };
        db.Participants.Add(participant); await db.SaveChangesAsync();
        return StatusCode(201, new { userId = participant.ParticipantId, participant.FirstName, participant.LastName, participant.Email, role });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var organiser = await db.Organisers.SingleOrDefaultAsync(x => x.Email.ToLower() == email);
        if (organiser is not null && PasswordService.Verify(request.Password, organiser.Password))
        {
            await SetSession(organiser.OrganiserId, "Organiser", organiser.Email, $"{organiser.FirstName} {organiser.LastName}");
            return Ok(new { message = "Login successful.", userId = organiser.OrganiserId, role = "Organiser", organiser.Email });
        }
        var participant = await db.Participants.SingleOrDefaultAsync(x => x.Email.ToLower() == email);
        if (participant is not null && PasswordService.Verify(request.Password, participant.Password))
        {
            await SetSession(participant.ParticipantId, "Participant", participant.Email, $"{participant.FirstName} {participant.LastName}");
            return Ok(new { message = "Login successful.", userId = participant.ParticipantId, role = "Participant", participant.Email });
        }
        return Unauthorized(new { message = "Incorrect email or password." });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() { HttpContext.Session.Clear(); return NoContent(); }

    private async Task SetSession(int id, string role, string email, string fullName)
    {
        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("UserId", id);
        HttpContext.Session.SetString("Role", role);
        HttpContext.Session.SetString("Email", email);
        HttpContext.Session.SetString("FullName", fullName);
        await HttpContext.Session.CommitAsync();
    }
}
