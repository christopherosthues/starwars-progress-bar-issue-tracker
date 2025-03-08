using StarWarsProgressBarIssueTracker.App.Commons;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Issues;

public class IssueDto : DtoBase
{
    public required string Title { get; set; }

    public IssueState State { get; set; }

    public Priority Priority { get; set; }

    public IssueMilestoneDto? Milestone { get; set; }

    public IssueReleaseDto? Release { get; set; }

    public IList<IssueLabelDto> Labels { get; set; } = [];

    public string? Description { get; set; }

    public Vehicle? Vehicle { get; set; }

    public IList<IssueLinkDto> LinkedIssues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
