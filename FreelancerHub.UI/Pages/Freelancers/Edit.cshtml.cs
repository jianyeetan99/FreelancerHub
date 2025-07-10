using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FreelancerHub.UI.Models;
using FreelancerHub.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancerHub.UI.Pages.Freelancers;

[Authorize]
public class EditModel(IHttpClientFactory httpClientFactory, TokenStorage tokenStorage) : PageModel
{
    [BindProperty] public Guid Id { get; set; }
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string PhoneNumber { get; set; } = "";
    [BindProperty] public string SkillsText { get; set; } = "";
    [BindProperty] public string HobbiesText { get; set; } = "";
    public List<string> Skills => SkillsText.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
    public List<string> Hobbies => HobbiesText.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var client = httpClientFactory.CreateClient("Api");
        var token = tokenStorage.GetToken();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/freelancers/{id}");
        if (!response.IsSuccessStatusCode) return RedirectToPage("Index");

        var json = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<FreelancerDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Id = dto.Id;
        Username = dto.Username;
        Email = dto.Email;
        PhoneNumber = dto.PhoneNumber;
        SkillsText = string.Join(", ", dto.Skills);
        HobbiesText = string.Join(", ", dto.Hobbies);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateClient("Api");
        var token = tokenStorage.GetToken();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var data = new
        {
            Id,
            Username,
            Email,
            PhoneNumber,
            Skills,
            Hobbies 
        };

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/api/freelancers/{Id}", content);

        return RedirectToPage("Index");
    }
}