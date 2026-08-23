using System.ComponentModel.DataAnnotations;

namespace CinemaMvc.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [StringLength(100)]
        public string FirstName{get;set;}="";

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = "";

        [Phone]
        public string? PhoneNumber { get; set; }

        public string ConcurrencyStamp { get; set; } = string.Empty;
        
    }

}

