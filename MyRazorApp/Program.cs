using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRazorApp;
using MyRazorApp.Data;
using MyRazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Database bağlantısı
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

// Identity ayarları
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
});

builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedUsersAndRolesAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Ana sayfa -> Login
app.MapGet("/", async context =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/Index");
    }
    else
    {
        context.Response.Redirect("/Login");
    }

    await Task.CompletedTask;
});


app.Run();
