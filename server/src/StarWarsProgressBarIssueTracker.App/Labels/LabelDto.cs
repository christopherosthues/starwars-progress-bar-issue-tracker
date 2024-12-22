namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Color { get; set; }

    public required string TextColor { get; set; }

    public string? GitlabId { get; set; }

    public string? GitHubId { get; set; }
}
