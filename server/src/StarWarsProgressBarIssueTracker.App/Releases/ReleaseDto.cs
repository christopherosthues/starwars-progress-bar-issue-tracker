using StarWarsProgressBarIssueTracker.App.Commons;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Releases;

public class ReleaseDto : DtoBase
{
    public required string Title { get; set; }

    public string? Notes { get; set; }

    public ReleaseState State { get; set; }

    public DateTime? Date { get; set; }

    public IList<ReleaseIssueDto> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
