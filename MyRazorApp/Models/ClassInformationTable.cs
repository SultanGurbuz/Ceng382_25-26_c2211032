using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public required List<Class> FilteredClasses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public static ClassInformationTable CreateFilteredTable(List<Class> allClasses, string filter, int page, int pageSize)
        {
            var filtered = string.IsNullOrEmpty(filter) 
                ? allClasses 
                : allClasses.Where(c => c.Name.Contains(filter) || c.Description.Contains(filter)).ToList();

            var totalPages = (int)Math.Ceiling((double)filtered.Count / pageSize);
            var paginated = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new ClassInformationTable
            {
                FilteredClasses = paginated,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }
}