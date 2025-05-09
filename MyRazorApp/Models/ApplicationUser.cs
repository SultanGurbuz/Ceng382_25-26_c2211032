using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
