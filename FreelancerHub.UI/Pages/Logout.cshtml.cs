using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly TokenStorage _tokenStorage;

    public LogoutModel(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }
    
    public IActionResult OnGet()
    {
        Response.Cookies.Delete("X-Access-Token");
        return RedirectToPage("/Login");
    }
}