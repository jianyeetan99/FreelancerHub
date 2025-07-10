using FreelancerHub.Application.DTOs;

namespace FreelancerHub.Application.Interfaces;

public interface IFreelancerService
{
    Task<List<FreelancerDto>> GetAllAsync();
    Task<FreelancerDto?> GetByIdAsync(Guid id);
    Task<List<FreelancerDto>> SearchAsync(string keyword);
    Task<Guid> CreateAsync(CreateFreelancerDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateFreelancerDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<bool> UnarchiveAsync(Guid id);
}