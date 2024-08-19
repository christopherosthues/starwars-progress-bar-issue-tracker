using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public class Appearance : DomainBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Color { get; set; }

    public required string TextColor { get; set; }

    public string? GitlabId { get; set; }

    public string? GitHubId { get; set; }
}
