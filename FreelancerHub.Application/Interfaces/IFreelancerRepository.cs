using FreelancerHub.Domain.Entities;

namespace FreelancerHub.Application.Interfaces;

public interface IFreelancerRepository
{
    Task<List<Freelancer>> GetAllAsync();
    Task<Freelancer?> GetByIdAsync(Guid id);
    Task<List<Freelancer>> SearchAsync(string keyword);
    Task<Guid> CreateAsync(Freelancer freelancer);
    Task<bool> UpdateAsync(Freelancer freelancer);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<bool> UnarchiveAsync(Guid id);
}