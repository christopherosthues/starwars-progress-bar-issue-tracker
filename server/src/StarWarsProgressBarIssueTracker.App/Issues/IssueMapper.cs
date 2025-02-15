using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Issues;

public class IssueMapper
{
    public IssueDto? MapToNullableIssueDto(Issue? issue)
    {
        return issue == null ? null : MapToIssueDto(issue);
    }

    public IssueDto MapToIssueDto(Issue issue)
    {
        return new IssueDto
        {
            Id = issue.Id,
            CreatedAt = issue.CreatedAt,
            LastModifiedAt = issue.LastModifiedAt,
            Title = issue.Title,
            Description = issue.Description,
            State = issue.State,
            Priority = issue.Priority,
            Milestone = MapToMilestoneDto(issue.Milestone),
            Release = MapToReleaseDto(issue.Release),
            Vehicle = issue.Vehicle,
            Labels = issue.Labels.Select(MapToIssueLabelDto).ToList(),
            LinkedIssues = issue.LinkedIssues.Select(MapToIssueLinkDto).ToList(),
            GitlabId = issue.GitlabId,
            GitlabIid = issue.GitlabIid,
            GitHubId = issue.GitlabId,

        };
    }

    private static IssueLinkDto MapToIssueLinkDto(IssueLink link)
    {
        return new IssueLinkDto
        {
            Id = link.Id,
            CreatedAt = link.CreatedAt,
            LastModifiedAt = link.LastModifiedAt,
            Type = link.Type,
            LinkedIssue = new LinkedIssueDto
            {
                Id = link.LinkedIssue.Id,
                CreatedAt = link.LinkedIssue.CreatedAt,
                LastModifiedAt = link.LinkedIssue.LastModifiedAt,
                Title = link.LinkedIssue.Title,
                Description = link.LinkedIssue.Description,
                State = link.LinkedIssue.State,
                Priority = link.LinkedIssue.Priority,
                Milestone = MapToMilestoneDto(link.LinkedIssue.Milestone),
                Release = MapToReleaseDto(link.LinkedIssue.Release),
                Vehicle = link.LinkedIssue.Vehicle,
                GitlabId = link.LinkedIssue.GitlabId,
                GitlabIid = link.LinkedIssue.GitlabIid,
                GitHubId = link.LinkedIssue.GitlabId,
            }
        };
    }

    private static IssueMilestoneDto? MapToMilestoneDto(Milestone? milestone)
    {
        if (milestone == null)
        {
            return null;
        }

        return new IssueMilestoneDto
        {
            Id = milestone.Id,
            CreatedAt = milestone.CreatedAt,
            LastModifiedAt = milestone.LastModifiedAt,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            GitlabId = milestone.GitlabId,
            GitlabIid = milestone.GitlabIid,
            GitHubId = milestone.GitHubId,
        };
    }

    private static IssueReleaseDto? MapToReleaseDto(Release? release)
    {
        if (release == null)
        {
            return null;
        }

        return new IssueReleaseDto
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            LastModifiedAt = release.LastModifiedAt,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            Date = release.Date,
            GitlabId = release.GitlabId,
            GitlabIid = release.GitlabIid,
            GitHubId = release.GitlabId,
        };
    }

    private static IssueLabelDto MapToIssueLabelDto(Label label)
    {
        return new IssueLabelDto
        {
            Id = label.Id,
            CreatedAt = label.CreatedAt,
            LastModifiedAt = label.LastModifiedAt,
            Title = label.Title,
            Description = label.Description,
            Color = label.Color,
            TextColor = label.TextColor,
            GitHubId = label.GitHubId,
            GitlabId = label.GitlabId,
        };
    }

    public Issue MapToIssue(IssueDto issue)
    {
        return new Issue
        {
            Id = issue.Id,
            CreatedAt = issue.CreatedAt,
            LastModifiedAt = issue.LastModifiedAt,
            Title = issue.Title,
            Description = issue.Description,
            State = issue.State,
            Priority = issue.Priority,
            Milestone = MapToMilestone(issue.Milestone),
            Release = MapToRelease(issue.Release),
            Vehicle = issue.Vehicle,
            Labels = issue.Labels.Select(label => new Label
            {
                Id = label.Id,
                CreatedAt = label.CreatedAt,
                LastModifiedAt = label.LastModifiedAt,
                Title = label.Title,
                Description = label.Description,
                Color = label.Color,
                TextColor = label.TextColor,
                GitHubId = label.GitHubId,
                GitlabId = label.GitlabId,
            }).ToList(),
            GitlabId = issue.GitlabId,
            GitlabIid = issue.GitlabIid,
            GitHubId = issue.GitlabId,
        };
    }

    private static Milestone? MapToMilestone(IssueMilestoneDto? milestone)
    {
        if (milestone == null)
        {
            return null;
        }

        return new Milestone
        {
            Id = milestone.Id,
            CreatedAt = milestone.CreatedAt,
            LastModifiedAt = milestone.LastModifiedAt,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            GitlabId = milestone.GitlabId,
            GitlabIid = milestone.GitlabIid,
            GitHubId = milestone.GitHubId,
        };
    }

    private static Release? MapToRelease(IssueReleaseDto? release)
    {
        if (release == null)
        {
            return null;
        }

        return new Release
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            LastModifiedAt = release.LastModifiedAt,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            Date = release.Date,
            GitlabId = release.GitlabId,
            GitlabIid = release.GitlabIid,
            GitHubId = release.GitlabId,
        };
    }
}
