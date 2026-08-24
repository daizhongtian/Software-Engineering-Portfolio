using CinemaMvc.Data;
using CinemaMvc.Models;
using CinemaMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CinemaMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View("~/Views/Users/Index.cshtml", users);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var vm = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty
            };

            ViewBag.UserId = user.Id;
            ViewBag.Email = user.Email;
            return View("~/Views/Users/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var invalidUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                ViewBag.UserId = id;
                ViewBag.Email = invalidUser?.Email;
                return View("~/Views/Users/Edit.cshtml", vm);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                TempData["Error"] = "User was already deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Entry(user)
                .Property(u => u.ConcurrencyStamp)
                .OriginalValue = vm.ConcurrencyStamp;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var databaseUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (databaseUser == null)
                {
                    TempData["Error"] = "User was already deleted.";
                    return RedirectToAction(nameof(Index));
                }

                vm.FirstName = databaseUser.FirstName;
                vm.LastName = databaseUser.LastName;
                vm.PhoneNumber = databaseUser.PhoneNumber;
                vm.ConcurrencyStamp = databaseUser.ConcurrencyStamp ?? string.Empty;
                ViewBag.UserId = databaseUser.Id;
                ViewBag.Email = databaseUser.Email;
                ModelState.Clear();
                ModelState.AddModelError("", "User data changed. Reload page and try again.");
                return View("~/Views/Users/Edit.cshtml", vm);
            }

        }
        

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var vm = new DeleteUserViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty
            };

            return View("~/Views/Users/Delete.cshtml", vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteUserViewModel vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == vm.Id);
            if (user == null)
            {
                TempData["Error"] = "User was already deleted";
                return RedirectToAction(nameof(Index));
            }

            _context.Entry(user)
                .Property(u => u.ConcurrencyStamp)
                .OriginalValue = vm.ConcurrencyStamp;
            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var databaseUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == vm.Id);
                if (databaseUser == null)
                {
                    TempData["Error"] = "User was already deleted";
                    return RedirectToAction(nameof(Index));
                }

                vm.FullName = $"{databaseUser.FirstName} {databaseUser.LastName}".Trim();
                vm.ConcurrencyStamp = databaseUser.ConcurrencyStamp ?? string.Empty;
                ModelState.Clear();
                ModelState.AddModelError("", "User data changed. Reload page and try again.");
                return View("~/Views/Users/Delete.cshtml", vm);
            }

        }

    }
}
