namespace FreelancerHub.Application.DTOs;

public class CreateFreelancerDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public List<string> Skills { get; set; } = new();
    public List<string> Hobbies { get; set; } = new();
}