using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
//prompt: Please write the code in C# for a Razor Page that handles user logout. The page should clear the session variables and cookies set during login. After logging out, it should redirect the user to the login page.
namespace MyRazorApp.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");
            return await Task.FromResult(RedirectToPage("Login"));
        }
    }
}