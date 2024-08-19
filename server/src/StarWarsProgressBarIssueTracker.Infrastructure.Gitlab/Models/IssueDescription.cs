using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Models;

public class IssueDescription
{
    public string? Description { get; set; }

    public int Priority { get; set; }

    public string? EngineColor { get; set; }

    public IEnumerable<Translation> Translations { get; set; } = new List<Translation>();
}
