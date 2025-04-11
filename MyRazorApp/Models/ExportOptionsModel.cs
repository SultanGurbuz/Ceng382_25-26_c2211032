
namespace MyRazorApp.Models
{
    public class ExportOptionsModel
    {
        public bool ExportOnlyFiltered { get; set; } = false;
        public bool ExportClassName { get; set; } = false;
        public bool ExportStudentCount { get; set; } = false;
        public bool ExportClassDescription { get; set; } = false;
       

    }
}