using CinemaMvc.Data;
using CinemaMvc.Dtos;
using CinemaMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMvc.Controllers.API;

[ApiController]
[Route("api/screenings")]
public class ScreeningsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScreeningsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScreeningDto>>> GetScreenings()
    {
        var screenings = await _context.Screenings
            .Include(s => s.Cinema)
            .OrderBy(s => s.StartTime)
            .Select(s => new ScreeningDto
            {
                Id = s.Id,
                FilmTitle = s.FilmTitle,
                StartTime = s.StartTime,
                CinemaId = s.CinemaId,
                CinemaName = s.Cinema != null ? s.Cinema.Name : ""
            })
            .ToListAsync();

        return screenings;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScreeningDto>> GetScreening(int id)
    {
        var screening = await _context.Screenings
            .Include(s => s.Cinema)
            .Where(s => s.Id == id)
            .Select(s => new ScreeningDto
            {
                Id = s.Id,
                FilmTitle = s.FilmTitle,
                StartTime = s.StartTime,
                CinemaId = s.CinemaId,
                CinemaName = s.Cinema != null ? s.Cinema.Name : ""
            })
            .FirstOrDefaultAsync();

        if (screening == null)
        {
            return NotFound();
        }

        return screening;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ScreeningDto>> CreateScreening(CreateScreeningDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FilmTitle))
        {
            return BadRequest("Film title is required.");
        }

        var cinema = await _context.Cinemas.FindAsync(dto.CinemaId);
        if (cinema == null)
        {
            return BadRequest("Cinema does not exist.");
        }

        var screening = new Screening
        {
            FilmTitle = dto.FilmTitle.Trim(),
            StartTime = dto.StartTime,
            CinemaId = dto.CinemaId
        };

        _context.Screenings.Add(screening);
        await _context.SaveChangesAsync();

        var result = new ScreeningDto
        {
            Id = screening.Id,
            FilmTitle = screening.FilmTitle,
            StartTime = screening.StartTime,
            CinemaId = screening.CinemaId,
            CinemaName = cinema.Name
        };

        return CreatedAtAction(nameof(GetScreening), new { id = screening.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteScreening(int id)
    {
        var screening = await _context.Screenings.FindAsync(id);
        if (screening == null)
        {
            return NotFound();
        }

        _context.Screenings.Remove(screening);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
