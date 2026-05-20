using CinemaMvc.Dtos;
using CinemaMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaMvc.Controllers.API;

[ApiController]
[Route("api/account")]
public class AccountApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return new CurrentUserDto
            {
                IsAuthenticated = false,
                Email = null,
                Roles = Array.Empty<string>()
            };
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return new CurrentUserDto
            {
                IsAuthenticated = false,
                Email = null,
                Roles = Array.Empty<string>()
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto
        {
            IsAuthenticated = true,
            Email = user.Email,
            Roles = roles.ToArray()
        };
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<CurrentUserDto>> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            return BadRequest("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            return BadRequest("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email.Trim(),
            Email = dto.Email.Trim(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? null
                : dto.PhoneNumber.Trim()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(error => error.Description));
            return BadRequest(errors);
        }

        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto
        {
            IsAuthenticated = true,
            Email = user.Email,
            Roles = roles.ToArray()
        };
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}
