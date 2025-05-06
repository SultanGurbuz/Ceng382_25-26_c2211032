
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class Class     // prompt: "Create a Class model with properties for Id, Name, StudentCount, Description, and IsActive. Use appropriate data annotations for validation."
    {
        [Key] 
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; } = null!;
        [Required] 
        [Range(0, 100)] 
        public int StudentCount { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required] 
        public bool IsActive { get; set; }
    }
}