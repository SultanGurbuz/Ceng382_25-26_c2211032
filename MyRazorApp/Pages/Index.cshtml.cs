using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private const string SessionKeyClassList = "ClassList";

        [BindProperty]
        public ClassInformationModel ClassInformation { get; set; } = new ClassInformationModel
        {
            ClassName = string.Empty,
            ClassDescription = string.Empty
        };

        public List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        public List<ClassInformationTableModel> ClassListTable { get; set; } = new List<ClassInformationTableModel>();

        public bool IsEdit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterBy { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }


        public IndexModel()
        {
            FilterBy = string.Empty;
        }

        private void GenerateSyntheticData()
        {
            if (ClassList.Count == 0)
            {
                for (int i = 1; i <= 100; i++)
                {
                    ClassList.Add(ClassInformationModel.Create(
                        $"Class {i}",
                        new Random().Next(10, 101),
                        $"Description for Class {i}"
                    ));
                }
                SaveClassListToSession();
            }
        }

        public void OnGet()
        {
            LoadClassListFromSession();

            if (ClassList.Count == 0)
            {
                GenerateSyntheticData();
            }

            IEnumerable<ClassInformationModel> filteredList = ClassList;

            // Apply filtering
            if (!string.IsNullOrEmpty(FilterBy))
            {
                filteredList = ClassList.Where(c =>
                    c.ClassName.Contains(FilterBy, StringComparison.OrdinalIgnoreCase) ||
                    c.ClassDescription.Contains(FilterBy, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Apply pagination
            TotalPages = (int)Math.Ceiling(filteredList.Count() / (double)PageSize);
            var pagedList = filteredList.Skip((PageNumber - 1) * PageSize).Take(PageSize);

            // Map to ClassInformationTableModel
            ClassListTable = pagedList.Select(c => new ClassInformationTableModel
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                ClassDescription = c.ClassDescription,
            }).ToList();
        }

        public IActionResult OnPostAdd()
        {
            LoadClassListFromSession();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input. Please check the form fields.");
                return Page();
            }

            if (ClassList.Any(c => c.ClassName == ClassInformation.ClassName))
            {
                ModelState.AddModelError("ClassInformation.ClassName", "This class already exists!");
                return Page();
            }

            var newClass = new ClassInformationModel
            {
                Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1,
                ClassName = ClassInformation.ClassName,
                StudentCount = ClassInformation.StudentCount,
                ClassDescription = ClassInformation.ClassDescription
            };

            ClassList.Add(newClass);
            SaveClassListToSession();

            // Debugging log
            Console.WriteLine("Class added successfully: " + JsonSerializer.Serialize(newClass));

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostDelete(int id)
        {
            LoadClassListFromSession();

            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            SaveClassListToSession();

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            LoadClassListFromSession();

            if (ClassList == null || !ClassList.Any())
            {
                ModelState.AddModelError(string.Empty, "Class list is empty. Please try again.");
                return Page();
            }

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
            else
            {
                IsEdit = false; // Reset IsEdit if the class is not found
            }

            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            LoadClassListFromSession();

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

            SaveClassListToSession();

            // Debugging log
            Console.WriteLine("Class updated successfully: " + JsonSerializer.Serialize(classToUpdate));

            IsEdit = false; // Reset IsEdit after updating
            return RedirectToPage("./Index");
        }

        private void LoadClassListFromSession()
        {
            ClassList = HttpContext.Session.GetObjectFromJson<List<ClassInformationModel>>(SessionKeyClassList) ?? new List<ClassInformationModel>();

            // Debugging log
            Console.WriteLine("Loaded ClassList from session: " + JsonSerializer.Serialize(ClassList));
        }

        private void SaveClassListToSession()
        {
            HttpContext.Session.SetObjectAsJson(SessionKeyClassList, ClassList);

            // Debugging log
            Console.WriteLine("Saved ClassList to session: " + JsonSerializer.Serialize(ClassList));
        }
    }
}

public static class SessionHelper
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}