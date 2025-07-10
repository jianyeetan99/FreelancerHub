using System.Net.Http.Headers;
using System.Text.Json;
using FreelancerHub.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Freelancers;

[Authorize]
public class IndexModel(IHttpClientFactory httpClientFactory, TokenStorage tokenStorage)
    : PageModel
{
    public List<FreelancerDto> Freelancers { get; set; } = new();
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateClient("Api");

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/freelancers");

        var token = tokenStorage.JwtToken;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Error = "Failed to load freelancers.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<FreelancerDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Freelancers = result ?? new();
        return Page();
    }

}