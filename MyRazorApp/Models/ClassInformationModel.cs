// Prompt: create a new class named ClassInformationModel in the Models folder and add the following properties to it:
// Id, ClassName, StudentCount, ClassDescription Id must auto Increment when a new object is created. add required and range properties
using System.ComponentModel.DataAnnotations;
namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 0;

        // Varsayılan yapıcı artık ID üretmiyor
        public ClassInformationModel()
        {
            ClassName = string.Empty;
            ClassDescription = string.Empty;
        }

        public int Id { get; set; }
        
        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }
        
        [Range(10, 100, ErrorMessage = "Student Count must be between 10 and 100.")]
        public int StudentCount { get; set; }
        
        [Required(ErrorMessage = "Class Description is required.")]
        public string ClassDescription { get; set; }
        
        // Yeni nesne oluştururken ID üretimi için statik metot
        public static ClassInformationModel Create(string className, int studentCount, string classDescription)
        {
            return new ClassInformationModel
            {
                Id = Interlocked.Increment(ref _idCounter),
                ClassName = className,
                StudentCount = studentCount,
                ClassDescription = classDescription
            };
        }
    }
}
