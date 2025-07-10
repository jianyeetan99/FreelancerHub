namespace FreelancerHub.Domain.Entities;

public class Skill
{
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = null!;
    public Guid SkillFreelancerId { get; set; }
}
