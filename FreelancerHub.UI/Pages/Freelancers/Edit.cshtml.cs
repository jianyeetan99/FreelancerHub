using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Freelancers;

[Authorize]
public class EditModel(IHttpClientFactory httpClientFactory, TokenStorage tokenStorage)
    : PageModel
{
    [BindProperty] public Guid Id { get; set; }
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Skill { get; set; } = string.Empty;
    [BindProperty] public decimal RatePerHour { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var client = httpClientFactory.CreateClient("Api");
        if (!string.IsNullOrEmpty(tokenStorage.JwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStorage.JwtToken);
        }

        var response = await client.GetAsync($"/api/freelancers/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Freelancers/Index");

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FreelancerResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result?.Data is null) return RedirectToPage("/Freelancers/Index");

        Id = result.Data.Id;
        Name = result.Data.Name;
        Skill = result.Data.Skill;
        RatePerHour = result.Data.RatePerHour;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateClient("Api");

        if (!string.IsNullOrEmpty(tokenStorage.JwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStorage.JwtToken);
        }

        var payload = new
        {
            Id,
            Name,
            Skill,
            RatePerHour
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"/api/freelancers/{Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Update failed.");
            return Page();
        }

        return RedirectToPage("/Freelancers/Index");
    }

    private class FreelancerResponse
    {
        public FreelancerDto? Data { get; set; }
    }

    private class FreelancerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Skill { get; set; } = string.Empty;
        public decimal RatePerHour { get; set; }
    }
}
