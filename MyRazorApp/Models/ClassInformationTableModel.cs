using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class ClassInformationTableModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public required string ClassName { get; set; }

        [Display(Name = "Student Count")]
        public int StudentCount { get; set; }

        [Required]
        [Display(Name = "Description")]
        public required string ClassDescription { get; set; }
    }
}