namespace FreelancerHub.Domain.Entities;

public class Hobby
{
    public Guid HobbyId { get; set; }
    public string HobbyName { get; set; } = null!;
    public Guid HobbyFreelancerId { get; set; }
}
