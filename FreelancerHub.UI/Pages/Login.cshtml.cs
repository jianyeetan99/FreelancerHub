using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreelancerHub.UI.Services;

namespace FreelancerHub.UI.Pages;

public class LoginModel(IHttpClientFactory httpFactory, TokenStorage tokenStorage) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpFactory.CreateClient("Api");

        var body = new
        {
            Email,
            Password
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/login", content);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        var result = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
        var token = result.GetProperty("accessToken").GetString();

        tokenStorage.SaveToken(token!);

        // await HttpContext.SignInAsync(new ClaimsPrincipal());

        return RedirectToPage("/Freelancers/Index");
    }
}