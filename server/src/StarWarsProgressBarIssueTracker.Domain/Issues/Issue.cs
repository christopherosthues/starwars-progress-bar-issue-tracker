using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Domain.Issues;

public class Issue : DomainBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public Priority Priority { get; set; }

    public IssueState State { get; set; }

    public Milestone? Milestone { get; set; }

    public Release? Release { get; set; }

    public Vehicle? Vehicle { get; set; }

    public IList<Label> Labels { get; set; } = [];

    public IList<IssueLink> LinkedIssues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
