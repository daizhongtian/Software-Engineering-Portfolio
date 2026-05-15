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
    public class profileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public profileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult>Edit()
        {
            var user =await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return NotFound();
            }
            var vm = new EditProfileViewModel
            {
                FirstName=user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty
            };
            return View(vm);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(EditProfileViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user ==null)
            {
                return NotFound();
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;
            _context.Entry(user)
                .Property(u => u.ConcurrencyStamp)
                .OriginalValue = vm.ConcurrencyStamp;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"]="Profile updated successfully";
                return RedirectToAction(nameof(Edit));
            }
            catch (DbUpdateConcurrencyException)
            {
                var databaseUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                if (databaseUser == null)
                {
                    return NotFound();
                }

                vm.FirstName = databaseUser.FirstName;
                vm.LastName = databaseUser.LastName;
                vm.PhoneNumber = databaseUser.PhoneNumber;
                vm.ConcurrencyStamp = databaseUser.ConcurrencyStamp ?? string.Empty;
                ModelState.Clear();
                ModelState.AddModelError("", "User data changed. Reload page and try again");
                return View(vm);
            }
        }
        


    }
}
