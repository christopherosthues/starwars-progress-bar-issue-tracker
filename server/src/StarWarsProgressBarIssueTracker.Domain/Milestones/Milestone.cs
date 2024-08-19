using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Milestones;

public class Milestone : DomainBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public MilestoneState State { get; set; }

    public IList<Issue> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
