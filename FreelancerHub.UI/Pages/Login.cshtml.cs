using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages;

public class LoginModel(IHttpClientFactory httpFactory, TokenStorage tokenStorage) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? Error { get; set; }
    public string? StatusMessage { get; set; }

    public void OnGet(bool registered = false)
    {
        if (registered)
        {
            StatusMessage = "Registration successful. Please log in.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpFactory.CreateClient("Api");

        var payload = new { Email, Password };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            Error = "Invalid login.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var token = result?.Token;

        if (string.IsNullOrEmpty(token))
        {
            Error = "Failed to retrieve token.";
            return Page();
        }

        tokenStorage.JwtToken = token;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Email),
            new Claim("AccessToken", token)
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CookieAuth", principal);

        return RedirectToPage("/Freelancers/Index");
    }

    private class LoginResult
    {
        public string? Token { get; set; }
    }
}
