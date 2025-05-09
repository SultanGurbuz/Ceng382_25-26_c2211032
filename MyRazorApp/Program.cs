using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

builder.Services.AddSession(o =>
{
    o.IdleTimeout          = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly      = true;
    o.Cookie.IsEssential   = true;
    o.Cookie.SameSite      = SameSiteMode.Strict;
});

// ── Pipeline ──────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// tiny security headers middleware
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    ctx.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

// Seed dummy class data once
using (var scope = app.Services.CreateScope())
{
    var db  = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
    var fn  = Path.Combine(app.Environment.WebRootPath, "data", "classes.json");

    if (File.Exists(fn) && !db.Classes.Any())
    {
        var list = JsonSerializer.Deserialize<List<Class>>(await File.ReadAllTextAsync(fn));
        if (list?.Any() == true)
        {
            db.Classes.AddRange(list);
            await db.SaveChangesAsync();
        }
    }
}

// Razor Pages first; fallback root → /Login
app.MapRazorPages();
app.MapGet("/", ctx =>
{

    if (ctx.Request.Cookies.ContainsKey("username"))
        ctx.Response.Redirect("/Index");
    else
        ctx.Response.Redirect("/Login");
    return Task.CompletedTask;
});
app.Run();
