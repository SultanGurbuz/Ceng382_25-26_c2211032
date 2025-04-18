var builder = WebApplication.CreateBuilder(args);
//prompt: "Add authentication and authorization to the app"
builder.Services.AddRazorPages();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
//prompt: "Add security headers to prevent XSS and clickjacking attacks"
app.Use(async (context, next) => {
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
//prompt: "It should start in login page and redirect to login page if not logged in"
app.MapGet("/", context => {
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();