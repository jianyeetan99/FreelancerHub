using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FreelancerHub.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Freelancers;

[Authorize]
public class CreateModel(IHttpClientFactory httpClientFactory, TokenStorage tokenStorage)
    : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public List<string>? Skills { get; set; } = null;

    [BindProperty]
    public List<string>? Hobbies { get; set; } = null;
    
    [TempData]
    public string? StatusMessage { get; set; }


    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateClient("Api");

        var jwtToken = tokenStorage.GetToken();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        var payload = new
        {
            Username,
            Email,
            PhoneNumber,
            Skills,
            Hobbies
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/freelancers", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to create freelancer.");
            return Page();
        }
        StatusMessage = "Freelancer created successfully!";
        return RedirectToPage("/Freelancers/Index");
    }
}