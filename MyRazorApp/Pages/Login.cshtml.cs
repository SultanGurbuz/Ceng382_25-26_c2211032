using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public LoginModel(IWebHostEnvironment env) => _env = env;

        [BindProperty, Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();

    // JSON path (inside wwwroot/Data)
    var jsonPath = Path.Combine(_env.WebRootPath, "Data", "users.json");
    if (!System.IO.File.Exists(jsonPath))
    {
        ErrorMessage = "User data not found.";
        return Page();
    }

    var json = await System.IO.File.ReadAllTextAsync(jsonPath);
    var users = JsonSerializer.Deserialize<List<JsonUser>>(json);

    var user = users?.FirstOrDefault(u =>
        u.Username == Username &&
        u.IsActive &&
        u.Password == Password // test amaçlı düz şifre kontrolü
    );

    if (user == null)
    {
        ErrorMessage = "Username or password is incorrect.";
        return Page();
    }

    // session + cookie işlemleri
    var token = Guid.NewGuid().ToString("N");
    HttpContext.Session.SetString("username", user.Username);
    HttpContext.Session.SetString("token", token);
    HttpContext.Session.SetString("sid", HttpContext.Session.Id);

    var opts = new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddMinutes(30),
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        Secure = Request.IsHttps
    };
    Response.Cookies.Append("username", user.Username, opts);
    Response.Cookies.Append("token", token, opts);
    Response.Cookies.Append("sid", HttpContext.Session.Id, opts);

    return RedirectToPage("/Index");
}


        public class JsonUser
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = "User";
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; }
        }
    }
}
