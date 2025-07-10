using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient("Api");

        var payload = new
        {
            Username,
            Email,
            Password
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Login", new {registered = true});
        }

        var error = await response.Content.ReadAsStringAsync();
        ErrorMessage = $"Failed to register: {error}";

        return Page();
    }
}