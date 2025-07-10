using FreelancerHub.Application.DTOs;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Domain.Entities;

namespace FreelancerHub.Application.Services;

public class FreelancerService : IFreelancerService
{
    private readonly IFreelancerRepository _repository;

    public FreelancerService(IFreelancerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FreelancerDto>> GetAllAsync()
    {
        var freelancers = await _repository.GetAllAsync();
        return freelancers.Select(MapToDto).ToList();
    }

    public async Task<FreelancerDto?> GetByIdAsync(Guid id)
    {
        var freelancer = await _repository.GetByIdAsync(id);
        return freelancer == null ? null : MapToDto(freelancer);
    }

    public async Task<List<FreelancerDto>> SearchAsync(string keyword)
    {
        var freelancers = await _repository.SearchAsync(keyword);
        return freelancers.Select(MapToDto).ToList();
    }

    public async Task<Guid> CreateAsync(CreateFreelancerDto dto)
    {
        var freelancer = new Freelancer
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Skills = dto.Skills.Select(s => new Skill { SkillId = Guid.NewGuid(), SkillName = s }).ToList(),
            Hobbies = dto.Hobbies.Select(h => new Hobby { HobbyId = Guid.NewGuid(), HobbyName = h }).ToList()
        };

        return await _repository.CreateAsync(freelancer);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateFreelancerDto dto)
    {
        var freelancer = await _repository.GetByIdAsync(id);
        if (freelancer == null) return false;

        freelancer.Username = dto.Username;
        freelancer.Email = dto.Email;
        freelancer.PhoneNumber = dto.PhoneNumber;
        freelancer.Skills = dto.Skills.Select(s => new Skill { SkillId = Guid.NewGuid(), SkillName = s, SkillFreelancerId = id }).ToList();
        freelancer.Hobbies = dto.Hobbies.Select(h => new Hobby { HobbyId = Guid.NewGuid(), HobbyName = h, HobbyFreelancerId = id }).ToList();

        return await _repository.UpdateAsync(freelancer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        return await _repository.ArchiveAsync(id);
    }

    public async Task<bool> UnarchiveAsync(Guid id)
    {
        return await _repository.UnarchiveAsync(id);
    }

    private FreelancerDto MapToDto(Freelancer f) => new()
    {
        Id = f.Id,
        Username = f.Username,
        Email = f.Email,
        PhoneNumber = f.PhoneNumber,
        Skills = f.Skills.Select(s => s.SkillName).ToList(),
        Hobbies = f.Hobbies.Select(h => h.HobbyName).ToList(),
        IsArchived = f.IsArchived
    };
}
