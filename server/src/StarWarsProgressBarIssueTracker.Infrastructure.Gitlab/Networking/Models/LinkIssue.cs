using System.Text.Json.Serialization;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;

public class LinkIssue
{
    public int Iid { get; set; }
    public int Id { get; set; }

    [JsonPropertyName("issue_link_id")]
    public int IssueLinkId { get; set; }

    [JsonPropertyName("project_id")]
    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string State { get; set; } = null!;

    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; }
}
