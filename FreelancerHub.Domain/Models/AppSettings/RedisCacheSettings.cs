namespace FreelancerHub.Domain.Models.AppSettings;

public sealed record RedisCacheSettings
{
    public string InstanceName { get; init; } = null!;
}