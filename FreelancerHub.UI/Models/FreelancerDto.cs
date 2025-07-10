namespace FreelancerHub.UI.Models;

public class FreelancerDto
 {
     public Guid Id { get; set; }
     public string Username { get; set; } = string.Empty;
     public string Email { get; set; } = string.Empty;
     public string PhoneNumber { get; set; } = string.Empty;
     public List<string> Skills { get; set; } = new();
     public List<string> Hobbies { get; set; } = new();
     public bool IsArchived { get; set; }
 }