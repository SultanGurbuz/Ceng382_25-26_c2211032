using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Helpers;
using MyRazorApp.Models;
using Newtonsoft.Json;

namespace MyRazorApp.Pages
{
    public class ClassListModel : PageModel
    {
        private const string SessionKey = "ClassList";

        public List<ClassInformationModel> ClassList { get; set; } = new();

        public void OnGet()
        {
           
            ClassList = HttpContext.Session.GetObjectFromJson<List<ClassInformationModel>>(SessionKey) ?? new List<ClassInformationModel>();
        }

        public IActionResult OnPostAddClass(string className, int studentCount, string classDescription)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input. Please check the form fields.");
                return Page();
            }

       
            ClassList = HttpContext.Session.GetObjectFromJson<List<ClassInformationModel>>(SessionKey) ?? new List<ClassInformationModel>();

           
            var newClass = new ClassInformationModel
            {
                ClassName = className,
                StudentCount = studentCount,
                ClassDescription = classDescription
            };
            ClassList.Add(newClass);

          
            HttpContext.Session.SetObjectAsJson(SessionKey, ClassList);

            return RedirectToPage();
        }

        public IActionResult OnPostEditClass(string originalClassName, string newClassName, int newStudentCount, string newClassDescription)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input. Please check the form fields.");
                return Page();
            }

         
            ClassList = HttpContext.Session.GetObjectFromJson<List<ClassInformationModel>>(SessionKey) ?? new List<ClassInformationModel>();

         
            var classToEdit = ClassList.FirstOrDefault(c => c.ClassName == originalClassName);
            if (classToEdit != null)
            {
         
                classToEdit.ClassName = newClassName;
                classToEdit.StudentCount = newStudentCount;
                classToEdit.ClassDescription = newClassDescription;


                HttpContext.Session.SetObjectAsJson(SessionKey, ClassList);
            }

            return RedirectToPage();
        }
    }
}