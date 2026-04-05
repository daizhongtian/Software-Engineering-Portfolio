using CinemaMvc.Data;
using CinemaMvc.Models;
using CinemaMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CinemaMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View("~/Views/Users/Index.cshtml", users);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
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
                var invalidUser = await _userManager.FindByIdAsync(id);
                ViewBag.UserId = id;
                ViewBag.Email = invalidUser?.Email;
                return View("~/Views/Users/Edit.cshtml", vm);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User was already deleted.";
                return RedirectToAction(nameof(Index));
            }

            if (user.ConcurrencyStamp != vm.ConcurrencyStamp)
            {
                vm.ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty;
                ViewBag.UserId = user.Id;
                ViewBag.Email = user.Email;
                ModelState.AddModelError("", "User data changed. Reload page and try again.");
                return View("~/Views/Users/Edit.cshtml", vm);
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            vm.ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty;
            ViewBag.UserId = user.Id;
            ViewBag.Email = user.Email;
            return View("~/Views/Users/Edit.cshtml", vm);

        }
        

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
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
            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null)
            {
                TempData["Error"] = "User was already deleted";
                return RedirectToAction(nameof(Index));
            }

            if (user.ConcurrencyStamp != vm.ConcurrencyStamp)
            {
                vm.FullName = $"{user.FirstName} {user.LastName}".Trim();
                ModelState.AddModelError("", "User data changed. Reload page and try again.");
                return View("~/Views/Users/Delete.cshtml", vm);
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            vm.FullName = $"{user.FirstName} {user.LastName}".Trim();
            return View("~/Views/Users/Delete.cshtml", vm);
        }

    }
}
