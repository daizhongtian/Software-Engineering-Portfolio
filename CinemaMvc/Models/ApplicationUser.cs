
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace CinemaMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName{ get; set;}=string.Empty;
    }
}



