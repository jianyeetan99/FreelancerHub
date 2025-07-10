using System;
namespace FreelancerHub.Domain.Entities
{
	public class Freelancer
	{
		public Guid Id { get; set; }
		public string Username { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public bool IsArchived { get; set; } = false;
		public List<Skill> Skills { get; set; } = new();
		public List<Hobby> Hobbies { get; set; } = new();
	}
}

