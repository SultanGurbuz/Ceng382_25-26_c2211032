using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Helpers;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class ClassInformation { get; set; } = new();

        public List<ClassInformationTableModel> ClassListTable { get; set; } = new();

        public bool IsEdit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterBy { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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
            PageNumber = Math.Clamp(PageNumber, 1, TotalPages);

            ClassListTable = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTableModel
                {
                    Id = c.Id,
                    ClassName = c.Name ?? "",
                    StudentCount = c.StudentCount,
                    ClassDescription = c.Description ?? ""
                })
                .ToListAsync();
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

        // Inactivate class
        public async Task<IActionResult> OnPostInactivateAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                item.IsActive = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageNumber, FilterBy });
        }

        // Update class details
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
    }
}
