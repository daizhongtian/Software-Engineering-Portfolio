using CinemaMvc.Data;
using CinemaMvc.Dtos;
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

            if (string.IsNullOrWhiteSpace(dto.ConcurrencyStamp))
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
