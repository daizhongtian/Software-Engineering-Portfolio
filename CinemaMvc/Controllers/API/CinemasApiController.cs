using CinemaMvc.Data;
using CinemaMvc.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMvc.Controllers.API;

[ApiController]
[Route("api/cinemas")]
public class CinemasApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CinemasApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CinemaDto>>> GetCinemas()
    {
        var cinemas = await _context.Cinemas
            .OrderBy(c => c.Name)
            .Select(c => new CinemaDto
            {
                Id = c.Id,
                Name = c.Name,
                Rows = c.Rows,
                SeatsPerRow = c.SeatsPerRow
            })
            .ToListAsync();

        return cinemas;
    }
}