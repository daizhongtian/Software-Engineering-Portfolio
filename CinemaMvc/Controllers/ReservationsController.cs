using CinemaMvc.Data;
using CinemaMvc.Models;
using CinemaMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMvc.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Seats(int screeningId)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Reservations)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening?.Cinema == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var vm = new ScreeningSeatsViewModel
            {
                ScreeningId = screening.Id,
                FilmTitle = screening.FilmTitle,
                StartTime = screening.StartTime,
                CinemaName = screening.Cinema.Name,
                Rows = screening.Cinema.Rows,
                SeatsPerRow = screening.Cinema.SeatsPerRow,
                ReservedSeats = screening.Reservations
                    .Select(r => $"{r.RowNumber}_{r.SeatNumber}")
                    .ToHashSet(),
                MySeats = screening.Reservations
                    .Where(r => r.UserId == userId)
                    .Select(r => $"{r.RowNumber}_{r.SeatNumber}")
                    .ToHashSet()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int screeningId, int rowNumber, int seatNumber)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening?.Cinema == null)
                return NotFound();

            if (rowNumber < 1 || rowNumber > screening.Cinema.Rows ||
                seatNumber < 1 || seatNumber > screening.Cinema.SeatsPerRow)
            {
                TempData["Error"] = "Wrong seat number.";
                return RedirectToAction(nameof(Seats), new { screeningId });
            }

            var alreadyExists = await _context.Reservations.AnyAsync(r =>
                r.ScreeningId == screeningId &&
                r.RowNumber == rowNumber &&
                r.SeatNumber == seatNumber);

            if (alreadyExists)
            {
                TempData["Error"] = "This seat is already reserved.";
                return RedirectToAction(nameof(Seats), new { screeningId });
            }

            var reservation = new Reservation
            {
                ScreeningId = screeningId,
                RowNumber = rowNumber,
                SeatNumber = seatNumber,
                UserId = userId
            };

            _context.Reservations.Add(reservation);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Seat reserved.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Another user reserved this seat at the same time.";
            }

            return RedirectToAction(nameof(Seats), new { screeningId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int screeningId, int rowNumber, int seatNumber)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var reservation = await _context.Reservations.FirstOrDefaultAsync(r =>
                r.ScreeningId == screeningId &&
                r.RowNumber == rowNumber &&
                r.SeatNumber == seatNumber &&
                r.UserId == userId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction(nameof(Seats), new { screeningId });
            }

            _context.Reservations.Remove(reservation);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservation cancelled.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "This reservation was already changed or removed.";
            }

            return RedirectToAction(nameof(Seats), new { screeningId });
        }
    }
}
