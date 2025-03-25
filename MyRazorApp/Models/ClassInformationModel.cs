// Prompt: create a new class named ClassInformationModel in the Models folder and add the following properties to it:
// Id, ClassName, StudentCount, ClassDescription Id must auto Increment when a new object is created. add required and range properties

using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 1;

        public int Id { get; private set; }

        public ClassInformationModel()
        {
            Id = Interlocked.Increment(ref _idCounter);
            ClassName = string.Empty; // Initialize ClassName with a default value
            ClassDescription = string.Empty; // Initialize ClassDescription with a default value
        }

        [Required]
        public string ClassName { get; set; }
        
        [Range(10, 100)]
        public int StudentCount { get; set; }
        
        [Required]
        public string ClassDescription { get; set; }
    }
}
