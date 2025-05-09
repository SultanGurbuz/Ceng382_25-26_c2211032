using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Helpers;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Classes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class ClassInformation { get; set; } = new() { Description = string.Empty };

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
            if (!await _context.Classes.AnyAsync())
            {
                var random = new Random();
                var dummyClasses = Enumerable.Range(1, 100).Select(i => new Class
                {
                    Name = $"Class {i}",
                    StudentCount = random.Next(10, 100),
                    Description = $"Auto-generated description {i}",
                    IsActive = true
                }).ToList();

                await _context.Classes.AddRangeAsync(dummyClasses);
                await _context.SaveChangesAsync();
            }

            await LoadClassListTableAsync();
            return Page();
        }

        private async Task LoadClassListTableAsync()
        {
            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(FilterBy))
            {
                query = query.Where(c =>
                    c.Name.Contains(FilterBy) ||
                    c.Description.Contains(FilterBy));
            }

            var count = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            TotalPages = Math.Max(TotalPages, 1);
            PageNumber = Math.Clamp(PageNumber, 1, TotalPages);

            ClassListTable = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTableModel
                {
                    Id = c.Id,
                    ClassName = c.Name ?? string.Empty,
                    StudentCount = c.StudentCount,
                    ClassDescription = c.Description ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Classes.Add(ClassInformation);
            await _context.SaveChangesAsync();
            return RedirectToPage(new { PageNumber, FilterBy });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null && item.IsActive)
            {
                item.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class marked as inactive.";
            }
            return RedirectToPage(new { PageNumber, FilterBy });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                ClassInformation = item;
                IsEdit = true;
            }

            await LoadClassListTableAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                IsEdit = true;
                await LoadClassListTableAsync();
                return Page();
            }

            var item = await _context.Classes.FindAsync(ClassInformation.Id);
            if (item != null)
            {
                item.Name = ClassInformation.Name;
                item.StudentCount = ClassInformation.StudentCount;
                item.Description = ClassInformation.Description;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { PageNumber, FilterBy });
        }

        public async Task<IActionResult> OnPostExportJsonAsync()
        {
            var query = _context.Classes.Where(c => c.IsActive);

            if (ExportOptions.ExportOnlyFiltered && !string.IsNullOrEmpty(FilterBy))
                query = query.Where(c => c.Name.Contains(FilterBy) || c.Description.Contains(FilterBy));

            var data = await query.ToListAsync();

            var selectedCols = Request.Form["SelectedCols"]
                                .ToString()
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var flags = selectedCols.Count > 0
                ? new Dictionary<string, bool>
                {
                    ["Name"] = selectedCols.Contains("Name"),
                    ["StudentCount"] = selectedCols.Contains("StudentCount"),
                    ["Description"] = selectedCols.Contains("Description")
                }
                : new Dictionary<string, bool>
                {
                    ["Name"] = ExportOptions.ExportClassName,
                    ["StudentCount"] = ExportOptions.ExportStudentCount,
                    ["Description"] = ExportOptions.ExportClassDescription
                };

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "json");
            Directory.CreateDirectory(dir);
            var file = $"export_{Guid.NewGuid():N}.json";
            var path = Path.Combine(dir, file);

            try
            {
                Utils.Instance.ExportToJson(
                    data,
                    path,
                    Utils.Instance.CreatePropertySelector<Class>(flags));

                TempData["ExportSuccess"] = $"Exported {data.Count} record(s) to {file}";
            }
            catch (Exception ex)
            {
                TempData["ExportError"] = $"Failed to export data: {ex.Message}";
            }

            return RedirectToPage(new { PageNumber, FilterBy });
        }
    }
}
