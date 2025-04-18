using System;

namespace MyRazorApp.Models
{
    public class User
    {   
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}