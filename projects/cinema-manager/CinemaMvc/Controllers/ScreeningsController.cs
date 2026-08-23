using CinemaMvc.Data;
using CinemaMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CinemaMvc.Controllers
{
    public class ScreeningsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ScreeningsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>Index()
        {
            var screenings = await _context.Screenings.Include(s=>s.Cinema).OrderBy(s=>s.StartTime).ToListAsync();
            return View(screenings);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]

         public async Task<IActionResult> Create()
        {
            ViewBag.CinemaId = new SelectList(await _context.Cinemas.ToListAsync(), "Id", "Name");
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Screening screening)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CinemaId = new SelectList(await _context.Cinemas.ToListAsync(), "Id", "Name");
                return View(screening);
            }

            _context.Screenings.Add(screening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screening == null) return NotFound();

            return View(screening);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            if (screening == null) return NotFound();

            _context.Screenings.Remove(screening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}