using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRazorApp;
using MyRazorApp.Data;
using MyRazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

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

builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// ── Middleware ──────────────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // önce session
app.UseAuthentication(); // sonra auth
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

// ── Seed Users & Roles ─────────────────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedUsersAndRolesAsync(services);
}

// ── Endpoints ──────────────────────────────────────────────────────────────

app.MapRazorPages();

// Optional: Redirect root path to login or index based on cookie
app.MapGet("/", context =>
{
    if (context.Request.Cookies.ContainsKey("username"))
        context.Response.Redirect("/Index");
    else
        context.Response.Redirect("/Login");

    return Task.CompletedTask;
});

app.Run();
