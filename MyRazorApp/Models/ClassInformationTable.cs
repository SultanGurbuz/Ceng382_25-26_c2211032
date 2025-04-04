using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public required List<ClassInformationModel> FilteredClasses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public static ClassInformationTable CreateFilteredTable(List<ClassInformationModel> allClasses, string filter, int page, int pageSize)
        {
            var filtered = string.IsNullOrEmpty(filter) 
                ? allClasses 
                : allClasses.Where(c => c.ClassName.Contains(filter) || c.ClassDescription.Contains(filter)).ToList();

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