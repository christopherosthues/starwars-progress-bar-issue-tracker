namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelIssueDto
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? LastModifiedAt { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public string? GitlabId { get; init; }

    public string? GitlabIid { get; init; }

    public string? GitHubId { get; init; }
}
