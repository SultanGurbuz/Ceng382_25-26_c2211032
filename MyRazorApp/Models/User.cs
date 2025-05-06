using System;
using System.ComponentModel.DataAnnotations;
//prompt: "Create a User model class with properties for Id, Username, PasswordHash, Role, IsActive, and CreatedAt. Use appropriate data annotations for validation."
namespace MyRazorApp.Models //to be implemented 
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(60)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
