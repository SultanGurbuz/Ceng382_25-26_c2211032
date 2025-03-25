using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.Linq;

/*Step 3 – Page Functionality
• On the left side of the page, there will be a form that collects:
o Class Name
o Student Count
o Description
• On the right side, there will be a table that displays all the submitted class data.
• The table will have the following columns:
o Id
o Class Name
o Student Count
o Description
o Actions (Edit and Delete)
The data should be validated and added to a static list each time the form is submitted. The data will
then be displayed in the table.
Step 4 – Requirements and Constraints
• Use Bootstrap to create a responsive layout with two columns (form on the left, table on the
right).
• Use Razor Pages only; no JavaScript is allowed.
• All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel.
• Form validation should be done using C# attributes like [Required], [Range], etc.
• When editing, pre-fill the form with the selected item's data.
• After deletion or editing, refresh the page and update the table accordingly.*/
namespace MyRazorApp.Pages{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel ClassInformation { get; set; } = new ClassInformationModel
        {
            ClassName = string.Empty,
            ClassDescription = string.Empty
        };

        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        public bool IsEdit { get; set; }

        public void OnGet()
        {
            // No changes needed here
        }

        public IActionResult OnPostAdd()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    // Aynı sınıf adı kontrolü
    if (ClassList.Any(c => c.ClassName == ClassInformation.ClassName))
    {
        ModelState.AddModelError("ClassInformation.ClassName", "This class already exists!");
        return Page();
    }

    // Yalnızca ekleme işlemi yapıldığında yeni nesne üretimi gerçekleşir.
    var newClass = ClassInformationModel.Create(
        ClassInformation.ClassName, 
        ClassInformation.StudentCount, 
        ClassInformation.ClassDescription
    );

    ClassList.Add(newClass);

    return RedirectToPage();
}


        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInformation = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    ClassDescription = classToEdit.ClassDescription
                };
                IsEdit = true;
            }
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var classToUpdate = ClassList.FirstOrDefault(c => c.Id == ClassInformation.Id);
            if (classToUpdate != null)
            {
                classToUpdate.ClassName = ClassInformation.ClassName;
                classToUpdate.StudentCount = ClassInformation.StudentCount;
                classToUpdate.ClassDescription = ClassInformation.ClassDescription;
            }
            IsEdit = false;
            return RedirectToPage();
        }
    }
}

