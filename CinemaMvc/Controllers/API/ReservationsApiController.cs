using CinemaMvc.Data;
using CinemaMvc.Dtos;
using CinemaMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CinemaMvc.Controllers.API;

[ApiController]
[Route("api/screenings/{screeningId:int}/seats")]
public class ReservationsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReservationsApiController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<ScreeningSeatsDto>> GetSeats(int screeningId)
    {
        var screening = await _context.Screenings
            .Include(s => s.Cinema)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == screeningId);

        if (screening == null || screening.Cinema == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);

        var reservations = await _context.Reservations
            .AsNoTracking()
            .Where(r => r.ScreeningId == screeningId)
            .ToListAsync();

        var seats = new List<SeatDto>();

        for (var row = 1; row <= screening.Cinema.Rows; row++)
        {
            for (var seat = 1; seat <= screening.Cinema.SeatsPerRow; seat++)
            {
                var reservation = reservations.FirstOrDefault(r =>
                    r.RowNumber == row &&
                    r.SeatNumber == seat);

                seats.Add(new SeatDto
                {
                    RowNumber = row,
                    SeatNumber = seat,
                    IsReserved = reservation != null,
                    IsMine = reservation != null && reservation.UserId == currentUserId
                });
            }
        }

        return new ScreeningSeatsDto
        {
            ScreeningId = screening.Id,
            FilmTitle = screening.FilmTitle,
            StartTime = screening.StartTime,
            CinemaName = screening.Cinema.Name,
            Rows = screening.Cinema.Rows,
            SeatsPerRow = screening.Cinema.SeatsPerRow,
            Seats = seats
        };
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ReserveSeat(int screeningId, ReserveSeatDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var screening = await _context.Screenings
            .Include(s => s.Cinema)
            .FirstOrDefaultAsync(s => s.Id == screeningId);

        if (screening == null || screening.Cinema == null)
        {
            return NotFound();
        }

        if (dto.RowNumber < 1 || dto.RowNumber > screening.Cinema.Rows ||
            dto.SeatNumber < 1 || dto.SeatNumber > screening.Cinema.SeatsPerRow)
        {
            return BadRequest("Seat position is outside the cinema room.");
        }

        var reservation = new Reservation
        {
            ScreeningId = screeningId,
            RowNumber = dto.RowNumber,
            SeatNumber = dto.SeatNumber,
            UserId = userId,
            ReservedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return Conflict("This seat is already reserved.");
        }

        return NoContent();
    }

    [HttpDelete("{rowNumber:int}/{seatNumber:int}")]
    [Authorize]
    public async Task<IActionResult> CancelReservation(
        int screeningId,
        int rowNumber,
        int seatNumber)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r =>
                r.ScreeningId == screeningId &&
                r.RowNumber == rowNumber &&
                r.SeatNumber == seatNumber &&
                r.UserId == userId);

        if (reservation == null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqliteException &&
               sqliteException.SqliteErrorCode == 19;
    }
}