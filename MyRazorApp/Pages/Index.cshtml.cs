using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Helpers;
using MyRazorApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private const string SessionKeyClassList = "ClassList";
        private const string SessionKeyExportOpts = "ExportOptions";
        private const string SessionKeyIsEdit = "IsEdit";
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public ClassInformationModel ClassInformation { get; set; } = new();

        public List<ClassInformationModel> ClassList { get; set; } = new();
        public List<ClassInformationTableModel> ClassListTable { get; set; } = new();

        [BindProperty]
        public ExportOptionsModel ExportOptions { get; set; } = new();

        public bool IsEdit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterBy { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public IndexModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void OnGet()
        {
            LoadClassListFromSession();
            IsEdit = HttpContext.Session.GetObjectFromJson<bool?>(SessionKeyIsEdit) ?? false;

            // prompt: It must only generate data if there is no data in the list
            if (!ClassList.Any())
            {
                GenerateSyntheticData();
            }

            LoadClassListTable();
        }
        // Prompt: a method for generate synthetic data for the class list.
        private void GenerateSyntheticData()
        {
            var random = new Random();
            for (int i = 1; i <= 100; i++)
            {
                ClassList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = random.Next(10, 101),
                    ClassDescription = $"Description for Class {i}"
                });
            }
            SaveClassListToSession();
        }

        private void LoadClassListTable()
        {
            var filtered = string.IsNullOrEmpty(FilterBy)
                ? ClassList
                : ClassList.Where(c =>
                    c.ClassName.Contains(FilterBy, StringComparison.OrdinalIgnoreCase) ||
                    c.ClassDescription.Contains(FilterBy, StringComparison.OrdinalIgnoreCase));

            TotalPages = (int)Math.Ceiling(filtered.Count() / (double)PageSize);
            TotalPages = TotalPages == 0 ? 1 : TotalPages;
            PageNumber = Math.Clamp(PageNumber, 1, TotalPages);

            ClassListTable = filtered
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTableModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    ClassDescription = c.ClassDescription
                })
                .ToList();
        }

        public IActionResult OnGetToggleColumn(string columnName)
        {
            LoadClassListFromSession();

            switch (columnName.ToLower())
            {
                case "classname":
                    ExportOptions.ExportClassName = !ExportOptions.ExportClassName;
                    break;
                case "studentcount":
                    ExportOptions.ExportStudentCount = !ExportOptions.ExportStudentCount;
                    break;
                case "classdescription":
                    ExportOptions.ExportClassDescription = !ExportOptions.ExportClassDescription;
                    break;
            }

            HttpContext.Session.SetObjectAsJson(SessionKeyExportOpts, ExportOptions);
            return RedirectToPage(new { FilterBy, PageNumber });
        }
        //prompt: A method for exporting a file when the user clicks the export button.
        public IActionResult OnPostExportJson()
        {
            LoadClassListFromSession();

            bool exportAllColumns = !ExportOptions.ExportClassName 
                                    && !ExportOptions.ExportStudentCount 
                                    && !ExportOptions.ExportClassDescription;

            var dataToExport = ExportOptions.ExportOnlyFiltered && !string.IsNullOrEmpty(FilterBy)
                ? ClassList.Where(c =>
                    c.ClassName.Contains(FilterBy, StringComparison.OrdinalIgnoreCase) ||
                    c.ClassDescription.Contains(FilterBy, StringComparison.OrdinalIgnoreCase))
                : ClassList;

            var dir = Path.Combine(_environment.ContentRootPath, "json");
            Directory.CreateDirectory(dir);
            
            var fileName = $"export{Guid.NewGuid().ToString("N")[..8]}.json";
            var filePath = Path.Combine(dir, fileName);

            var flags = new Dictionary<string, bool>
            {
                ["ClassName"] = exportAllColumns || ExportOptions.ExportClassName,
                ["StudentCount"] = exportAllColumns || ExportOptions.ExportStudentCount,
                ["ClassDescription"] = exportAllColumns || ExportOptions.ExportClassDescription
            };

            Utils.Instance.ExportToJson(
                dataToExport,
                filePath,
                Utils.Instance.CreatePropertySelector<ClassInformationModel>(flags)
            );

            TempData["ExportSuccess"] = $"Exported {dataToExport.Count()} records to {fileName}";
            return RedirectToPage();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid) 
                return Page();

            LoadClassListFromSession();

            int newId = ClassList.Any() ? ClassList.Max(c => c.Id) + 1 : 1;
            ClassList.Add(new ClassInformationModel
            {
                Id = newId,
                ClassName = ClassInformation.ClassName,
                StudentCount = ClassInformation.StudentCount,
                ClassDescription = ClassInformation.ClassDescription
            });

            SaveClassListToSession();
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            LoadClassListFromSession();

            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
                SaveClassListToSession();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            LoadClassListFromSession();
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassInformation = item;
                IsEdit = true;
                HttpContext.Session.SetObjectAsJson(SessionKeyIsEdit, true);
            }
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                IsEdit = true;
                HttpContext.Session.SetObjectAsJson(SessionKeyIsEdit, true);
                LoadClassListFromSession(); // Prompt: Load the class list again to show validation errors
            }

            LoadClassListFromSession();
            var item = ClassList.FirstOrDefault(c => c.Id == ClassInformation.Id);
            if (item != null)
            {
                item.ClassName = ClassInformation.ClassName;
                item.StudentCount = ClassInformation.StudentCount;
                item.ClassDescription = ClassInformation.ClassDescription;
                SaveClassListToSession();
            }

            IsEdit = false;
            HttpContext.Session.Remove(SessionKeyIsEdit);
            return RedirectToPage();
        }

        private void LoadClassListFromSession()
        {
            ClassList = HttpContext.Session.GetObjectFromJson<List<ClassInformationModel>>(SessionKeyClassList)
                        ?? new List<ClassInformationModel>();
            ExportOptions = HttpContext.Session.GetObjectFromJson<ExportOptionsModel>(SessionKeyExportOpts)
                            ?? new ExportOptionsModel();
        }

        private void SaveClassListToSession()
        {
            HttpContext.Session.SetObjectAsJson(SessionKeyClassList, ClassList);
            HttpContext.Session.SetObjectAsJson(SessionKeyExportOpts, ExportOptions);
        }
    }
}