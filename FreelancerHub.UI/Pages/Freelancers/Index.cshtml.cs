using System.Net.Http.Headers;
using System.Text.Json;
using FreelancerHub.Application.DTOs;
using FreelancerHub.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Freelancers;

[Authorize]
public class IndexModel(IHttpClientFactory httpClientFactory, TokenStorage tokenStorage) : PageModel
{
    public List<FreelancerDto> Freelancers { get; set; } = new();
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateClient("Api");

        var jwtToken = tokenStorage.GetToken();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        var response = await client.GetAsync("/api/freelancers");
        if (!response.IsSuccessStatusCode)
        {
            Error = "Failed to load freelancers.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        Freelancers = JsonSerializer.Deserialize<List<FreelancerDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        return Page();
    }
}
