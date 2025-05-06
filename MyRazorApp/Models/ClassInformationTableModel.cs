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

        [Display(Name = "Description")]
        public string ClassDescription { get; set; }
                public bool IsClassNameSelected { get; set; } = true;
        public bool IsStudentCountSelected { get; set; } = true;
        public bool IsClassDescriptionSelected { get; set; } = true;
    }
}