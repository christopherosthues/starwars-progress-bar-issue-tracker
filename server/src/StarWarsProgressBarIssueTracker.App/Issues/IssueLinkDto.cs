using StarWarsProgressBarIssueTracker.Domain.Issues;

namespace StarWarsProgressBarIssueTracker.App.Issues;

public class IssueLinkDto : DtoBase
{
    public LinkType Type { get; set; }

    public required LinkedIssueDto LinkedIssue { get; set; }
}
