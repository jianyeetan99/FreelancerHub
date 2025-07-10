namespace FreelancerHub.Application.DTOs;

public class FreelancerDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public List<string> Skills { get; set; } = new();
    public List<string> Hobbies { get; set; } = new();
    public bool IsArchived { get; set; }
}