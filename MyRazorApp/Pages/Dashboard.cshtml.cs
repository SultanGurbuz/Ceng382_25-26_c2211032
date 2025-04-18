using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Helpers;
using MyRazorApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyRazorApp.Pages
{
    public class DashboardModel : PageModel  // Changed from IndexModel to DashboardModel
    {
        private const string SessionKeyClassList = "ClassList";
        private const string SessionKeyExportOpts = "ExportOptions";
        private const string SessionKeyIsEdit = "IsEdit";

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

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            LoadClassListFromSession();
            IsEdit = HttpContext.Session.GetObjectFromJson<bool?>(SessionKeyIsEdit) ?? false;

            if (!ClassList.Any())
            {
                GenerateSyntheticData();
            }

            LoadClassListTable();
            return await Task.FromResult(Page());
        }

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

        public async Task<IActionResult> OnGetToggleColumnAsync(string columnName)
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

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
            return await Task.FromResult(RedirectToPage("Dashboard", new { FilterBy, PageNumber }));  // Specify "Dashboard" explicitly
        }

        public async Task<IActionResult> OnPostExportJsonAsync()
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            LoadClassListFromSession();

            bool exportAllColumns = !ExportOptions.ExportClassName
                                    && !ExportOptions.ExportStudentCount
                                    && !ExportOptions.ExportClassDescription;

            var dataToExport = ExportOptions.ExportOnlyFiltered && !string.IsNullOrEmpty(FilterBy)
                ? ClassList.Where(c =>
                    c.ClassName.Contains(FilterBy, StringComparison.OrdinalIgnoreCase) ||
                    c.ClassDescription.Contains(FilterBy, StringComparison.OrdinalIgnoreCase))
                : ClassList;

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "json");
            Directory.CreateDirectory(dir);

            var fileName = $"export{Guid.NewGuid().ToString("N")[..8]}.json";
            var filePath = Path.Combine(dir, fileName);

            var flags = new Dictionary<string, bool>
            {
                ["ClassName"] = exportAllColumns || ExportOptions.ExportClassName,
                ["StudentCount"] = exportAllColumns || ExportOptions.ExportStudentCount,
                ["ClassDescription"] = exportAllColumns || ExportOptions.ExportClassDescription
            };

            try
            {
                Utils.Instance.ExportToJson(dataToExport, filePath, Utils.Instance.CreatePropertySelector<ClassInformationModel>(flags));
                TempData["ExportSuccess"] = $"Exported {dataToExport.Count()} records to {fileName}";
            }
            catch (Exception ex)
            {
                TempData["ExportError"] = $"Failed to export data: {ex.Message}";
            }

            return await Task.FromResult(RedirectToPage("Dashboard", new { FilterBy, PageNumber }));  // Specify "Dashboard" explicitly
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid)
            {
                LoadClassListFromSession();
                return await Task.FromResult(Page());
            }

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
            return await Task.FromResult(RedirectToPage("Dashboard", new { FilterBy, PageNumber }));  // Specify "Dashboard" explicitly
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            LoadClassListFromSession();

            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
                SaveClassListToSession();
            }

            return await Task.FromResult(RedirectToPage("Dashboard", new { FilterBy, PageNumber }));  // Specify "Dashboard" explicitly
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            LoadClassListFromSession();
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassInformation = item;
                IsEdit = true;
                HttpContext.Session.SetObjectAsJson(SessionKeyIsEdit, true);
            }
            LoadClassListTable();
            return await Task.FromResult(Page());
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid)
            {
                IsEdit = true;
                HttpContext.Session.SetObjectAsJson(SessionKeyIsEdit, true);
                LoadClassListFromSession();
                LoadClassListTable();
                return await Task.FromResult(Page());
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
            return await Task.FromResult(RedirectToPage("Dashboard", new { FilterBy, PageNumber }));  // Specify "Dashboard" explicitly
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

        private bool IsAuthenticated()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];

            return !string.IsNullOrEmpty(sessionUsername) &&
                   !string.IsNullOrEmpty(sessionToken) &&
                   sessionUsername == cookieUsername &&
                   sessionToken == cookieToken;
        }
    }
}