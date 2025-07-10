using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreelancerHub.UI.Services;

namespace FreelancerHub.UI.Pages;

public class LogoutModel(TokenStorage tokenStorage) : PageModel
{
    public IActionResult OnGet()
    {
        tokenStorage.ClearToken();
        return RedirectToPage("/Login");
    }
}