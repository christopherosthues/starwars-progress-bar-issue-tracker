using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Releases;

public class Release : DomainBase
{
    public required string Title { get; set; }

    public string? Notes { get; set; }

    public ReleaseState State { get; set; }

    public DateTime? Date { get; set; }

    public IList<Issue> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
