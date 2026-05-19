using CinemaMvc.Data;
using CinemaMvc.Dtos;
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

        [HttpGet("api/admin/users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersApi()
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    ConcurrencyStamp = u.ConcurrencyStamp ?? string.Empty
                })
                .ToListAsync();

            return users;
        }

        [HttpGet("api/admin/users/{id}")]
        public async Task<ActionResult<UserDto>> GetUserApi(string id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    ConcurrencyStamp = u.ConcurrencyStamp ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("api/admin/users/{id}")]
        public async Task<IActionResult> UpdateUserApi(string id, [FromBody] UpdateUserDto? dto)
        {
            if (dto == null)
            {
                return BadRequest("User data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return BadRequest("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest("Last name is required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Entry(user)
                .Property(u => u.ConcurrencyStamp)
                .OriginalValue = dto.ConcurrencyStamp;

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? null
                : dto.PhoneNumber.Trim();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("User data changed. Reload page and try again.");
            }

            return NoContent();
        }

        [HttpDelete("api/admin/users/{id}")]
        public async Task<IActionResult> DeleteUserApi(string id, [FromBody] DeleteUserDto? dto)
        {
            if (dto == null)
            {
                return BadRequest("Concurrency stamp is required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Entry(user)
                .Property(u => u.ConcurrencyStamp)
                .OriginalValue = dto.ConcurrencyStamp;

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("User data changed. Reload page and try again.");
            }

            return NoContent();
        }
    }
}
