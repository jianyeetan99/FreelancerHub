namespace FreelancerHub.Domain.Models.AppSettings;

public sealed record ConnectionStrings
{
    public string DefaultConnection { get; init; } = null!;
    public string Redis { get; init; } = null!;
}