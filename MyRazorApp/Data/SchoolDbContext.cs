using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;


namespace MyRazorApp.Data
{
public class SchoolDbContext : IdentityDbContext<ApplicationUser>
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options) { }

        public DbSet<Class> Classes { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}