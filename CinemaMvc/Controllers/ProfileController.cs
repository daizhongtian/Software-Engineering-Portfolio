

using CinemaMvc.Models;
using CinemaMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaMvc.Controllers
{
    [Authorize]
    public class profileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public profileController(UserManager<ApplicationUser> userManager)
        {
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
            var user = await _userManager.GetUserAsync(User);
            if(user ==null)
            {
                return NotFound();
            }
            if(user.ConcurrencyStamp!=vm.ConcurrencyStamp)
            {
                vm.ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty;
                ModelState.AddModelError("","User data changed. Reload page and try again");
                return View(vm);
            }
            

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                TempData["Message"]="Profile updated successfully";
                return RedirectToAction(nameof(Edit));

            }
            foreach(var error in result.Errors)
            {
                    ModelState.AddModelError("", error.Description);
            }
            vm.ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty;
             return View(vm);
        }
        


    }
}
