using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Text.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//prompt: Please write the code in C# for a Razor Page that handles user login. The page should validate the username and password against a JSON file containing user data. If the login is successful, it should set session variables and cookies for the user. If the login fails, it should display an error message.
namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

       public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/users.json");
        if (!System.IO.File.Exists(filePath))
        {
            ErrorMessage = "User data file not found.";
            return Page();
        }

        var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
        var users = JsonSerializer.Deserialize<List<User>>(jsonData);
        var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

        if (user != null)
        {
            var token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true, // Set to false for local testing if HTTPS is not configured
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("username", user.Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, options);

            return RedirectToPage("Dashboard");  // Updated to Dashboard
        }

        ErrorMessage = "Username or password is incorrect.";
        return Page();
}
    }
}