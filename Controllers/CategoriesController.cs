using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController, Route("api/categories")]
public class CategoriesController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet, AllowAnonymous] public async Task<IActionResult> GetAll() => Ok(await db.Categories.AsNoTracking().Include(x => x.Event).Select(x => new { x.CategoryId, x.EventId, name = x.CategoryName, x.MinimumAge, x.MaximumAge, x.Distance, eventName = x.Event!.EventName }).ToListAsync());
    [HttpGet("{id:int}"), AllowAnonymous] public async Task<IActionResult> Get(int id) { var c = await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id); return c is null ? NotFound() : Ok(c); }
    [HttpPost, Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        if (request.MinimumAge > request.MaximumAge) return BadRequest(new { message = "MinimumAge cannot exceed MaximumAge." });
        var ev = await db.Events.FindAsync(request.EventId); if (ev is null) return NotFound(new { message = "Event does not exist." });
        if (ev.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        var c = new Category { EventId = request.EventId, CategoryName = request.Name.Trim(), MinimumAge = request.MinimumAge, MaximumAge = request.MaximumAge, Distance = request.Distance };
        db.Categories.Add(c); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = c.CategoryId }, c);
    }
    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        var c = await db.Categories.Include(x => x.Event).FirstOrDefaultAsync(x => x.CategoryId == id); if (c is null) return NotFound();
        if (c.Event!.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        if (request.MinimumAge > request.MaximumAge) return BadRequest(new { message = "MinimumAge cannot exceed MaximumAge." });
        c.CategoryName = request.Name.Trim(); c.MinimumAge = request.MinimumAge; c.MaximumAge = request.MaximumAge; c.Distance = request.Distance;
        await db.SaveChangesAsync(); return Ok(c);
    }
    [HttpDelete("{id:int}"), Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id) { var c = await db.Categories.Include(x => x.Event).FirstOrDefaultAsync(x => x.CategoryId == id); if (c is null) return NotFound(); if (c.Event!.OrganiserId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid(); db.Categories.Remove(c); try { await db.SaveChangesAsync(); } catch (DbUpdateException) { return Conflict(new { message = "Category cannot be deleted while registrations use it." }); } return NoContent(); }
}
